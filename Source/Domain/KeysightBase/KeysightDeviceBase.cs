using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Domain.UnionTypes;

namespace Domain.KeysightBase
{
	public abstract class KeysightDeviceBase : IKeysightDeviceBase
	{
		public BehaviorSubject<TriggerState> TriggerStateBehaviourSubject { get; set; }
		public ConcurrentQueue<MeasurementValue> ReadingQueue { get; } = new ConcurrentQueue<MeasurementValue>();
		public Queue<double> GeneratorQueue { get; set; } = new Queue<double>();

		public abstract double CalculateNextMeasurementValue(double interference);

		protected KeysightDeviceBase()
		{
			TriggerStateBehaviourSubject = new BehaviorSubject<TriggerState>(TriggerState.Idle);

			TriggerStateBehaviourSubject
				.OfType<TriggerState.WaitForTrigger_>()
				.Subscribe(_ =>
				{
					var interference = GeneratorQueue.Dequeue();
					ReadingQueue.Enqueue(MeasurementValue.Double(CalculateNextMeasurementValue(interference)));
					GeneratorQueue.Enqueue(interference);
				});
		}
	}
}