// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.Scheduler.JobStores
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The type of exception used by a job store to report that another
	/// scheduler instance has concurrently modified the state of a system
	/// thereby causing the request to fail.
	/// </summary>
	[Serializable]
	public class ConcurrentModificationException : SchedulerException
	{
		/// <summary>
		/// Creates a concurrent modification exception.
		/// </summary>
		/// <param name="message">The exception message</param>
		public ConcurrentModificationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates a concurrent modification exception.
		/// </summary>
		/// <param name="message">The exception message</param>
		/// <param name="innerException">The inner exception</param>
		public ConcurrentModificationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Creates a concurrent modification exception from its serialized form.
		/// </summary>
		/// <param name="info">The serialization info</param>
		/// <param name="context">The streaming context</param>
		protected ConcurrentModificationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}