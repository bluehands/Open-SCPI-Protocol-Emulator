using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Domain.UnionTypes;

namespace Domain.KeysightBase
{
	public interface IKeysightDeviceBase
	{
		BehaviorSubject<TriggerState> TriggerStateBehaviourSubject { get; set; }
		ConcurrentQueue<MeasurementValue> ReadingQueue { get; }
		Queue<double> GeneratorQueue { get; set; }

		double CalculateNextMeasurementValue(double interference);
	}
}