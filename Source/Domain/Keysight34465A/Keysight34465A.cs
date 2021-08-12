using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.KeysightBase;
using Domain.UnionTypes;
using FunicularSwitch;
using Range = Domain.UnionTypes.Range;

namespace Domain.Keysight34465A
{
	public sealed class Keysight34465A : KeysightDeviceBase
	{
		private readonly Func<Keysight34465AConfiguration> configurationProvider;
		private static readonly TimeSpan DisplayVisibilityTime = TimeSpan.FromSeconds(5);

		private BehaviorSubject<MeasurementType> MeasurementTypeBehaviorSubject { get; }
		private BehaviorSubject<ElectricityType> ElectricityTypeBehaviorSubject { get; }
		private BehaviorSubject<Impedance> ImpedanceBehaviorSubject { get; }
		private BehaviorSubject<Range> RangeBehaviorSubject { get; }
		private BehaviorSubject<Resolution> ResolutionBehaviorSubject { get; }
		private BehaviorSubject<DisplayState> DisplayStateBehaviorSubject { get; }

		public Keysight34465A(Func<Keysight34465AConfiguration> configurationProvider)
		{
			this.configurationProvider = configurationProvider;
			MeasurementTypeBehaviorSubject = new BehaviorSubject<MeasurementType>(MeasurementType.Voltage);
			ElectricityTypeBehaviorSubject = new BehaviorSubject<ElectricityType>(ElectricityType.DC);
			ImpedanceBehaviorSubject = new BehaviorSubject<Impedance>(Impedance.Low);
			RangeBehaviorSubject = new BehaviorSubject<Range>(Range.Auto);
			ResolutionBehaviorSubject = new BehaviorSubject<Resolution>(Resolution.Def);
			DisplayStateBehaviorSubject = new BehaviorSubject<DisplayState>(DisplayState.Hidden);

			MeasurementTypeBehaviorSubject.Subscribe(measurementTypeValue =>
			{
				var list = measurementTypeValue.Match(
					_ =>
						configurationProvider().VoltageInterferenceFactors,
					_ =>
						configurationProvider().CurrentInterferenceFactors);
				GeneratorQueue = new Queue<double>(list);
			});
		}

		public Task<Result<ResponseValue>> GetIdentification()
		{
			return Task.FromResult(Result.Ok(ResponseValue.String(configurationProvider().Identification)));
		}

		public Task<Result<Unit>> Abort()
		{
			TriggerStateBehaviourSubject.OnNext(TriggerState.Idle);
			return Task.FromResult(Result.Ok(No.Thing));
		}

		public Task<Result<Unit>> Initiate()
		{
			ReadingQueue.Clear();
			TriggerStateBehaviourSubject.OnNext(TriggerState.WaitForTrigger);
			TriggerStateBehaviourSubject.OnNext(TriggerState.Idle);
			return Task.FromResult(Result.Ok(No.Thing));
		}

		public Task<Result<Unit>> Fetch(ConcurrentQueue<IByteArrayConvertible> queue)
		{
			foreach (var measurementValue in ReadingQueue)
			{
				queue.Enqueue(measurementValue);
			}
			return Task.FromResult(Result.Ok(No.Thing));
		}

		public Task<Result<Unit>> ConfigureCurrent(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution)
		{
			MeasurementTypeBehaviorSubject.OnNext(MeasurementType.Current);
			ElectricityTypeBehaviorSubject.OnNext(electricityType);
			RangeBehaviorSubject.OnNext(range.Match(
				some => some,
				() => Range.Auto));
			ResolutionBehaviorSubject.OnNext(resolution.Match(
				some => some,
				() => Resolution.Def));
			return Task.FromResult(Result<Unit>.Ok(No.Thing));
		}

		public async Task<Result<Unit>> DisplayText(string text)
		{
			DisplayStateBehaviorSubject.OnNext(DisplayState.DisplayText(text)); 
			await Task.Delay(DisplayVisibilityTime).ConfigureAwait(false);
			return Result.Ok(No.Thing);
		}

		public Task<Result<Unit>> ClearDisplay()
		{
			DisplayStateBehaviorSubject.OnNext(DisplayState.Hidden);
			return Task.FromResult(Result.Ok(No.Thing));
		}

		public Task<Result<Unit>> SetImpedance(Impedance impedance)
		{
			ImpedanceBehaviorSubject.OnNext(impedance);
			return Task.FromResult(Result.Ok(No.Thing));
		}

		public Task<Result<Unit>> ConfigureVoltage(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution)
		{
			MeasurementTypeBehaviorSubject.OnNext(MeasurementType.Voltage);
			ElectricityTypeBehaviorSubject.OnNext(electricityType);
			RangeBehaviorSubject.OnNext(range.Match(
				some => some,
				() => Range.Auto));
			ResolutionBehaviorSubject.OnNext(resolution.Match(some => some,
				() => Resolution.Def));
			return Task.FromResult(Result.Ok(No.Thing));
		}

		public IObservable<(ElectricityType, Impedance, Range, Resolution, TriggerState, DisplayState)> State =>
			ElectricityTypeBehaviorSubject.CombineLatest(
				ImpedanceBehaviorSubject,
				RangeBehaviorSubject,
				ResolutionBehaviorSubject,
				TriggerStateBehaviourSubject,
				DisplayStateBehaviorSubject);

		public override double CalculateNextMeasurementValue(double interference)
		{
			double GetImpedanceMultiplier()
			{
				return ImpedanceBehaviorSubject.Value.Match(
					_ => configurationProvider().HighImpedanceInterferenceMultiplier,
					_ => configurationProvider().LowImpedanceInterferenceMultiplier);
			}

			double GetRangeValue()
			{
				return MeasurementTypeBehaviorSubject.Value.Match(_ => RangeBehaviorSubject.Value.Match(
						number => number.Value,
						_ => configurationProvider().VoltageRangeAuto,
						_ => configurationProvider().VoltageRangeMin,
						_ => configurationProvider().VoltageRangeMax,
						_ => configurationProvider().VoltageRangeDef),
					_ => RangeBehaviorSubject.Value.Match(
						number => number.Value,
						_ => configurationProvider().CurrentRangeAuto,
						_ => configurationProvider().CurrentRangeMin,
						_ => configurationProvider().CurrentRangeMax,
						_ => configurationProvider().CurrentRangeDef));
			}

			var rangeValue = GetRangeValue();
			var impedanceMultiplier = GetImpedanceMultiplier();
			return rangeValue + interference * impedanceMultiplier * rangeValue;
		}
	}
}