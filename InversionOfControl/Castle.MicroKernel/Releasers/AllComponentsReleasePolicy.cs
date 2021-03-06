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

namespace Castle.MicroKernel.Releasers
{
	using System;
	using System.Collections;
	using System.Threading;

	[Serializable]
	public class AllComponentsReleasePolicy : IReleasePolicy
	{
		private readonly IDictionary instance2Burden =
			new Hashtable(new Util.ReferenceEqualityComparer());

		private readonly ReaderWriterLock rwLock = new ReaderWriterLock();

		public virtual void Track(object instance, Burden burden)
		{
			rwLock.AcquireWriterLock(Timeout.Infinite);
			try
			{
				instance2Burden[instance] = burden;
			}
			finally
			{
				rwLock.ReleaseWriterLock();
			}
		}

		public bool HasTrack(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			rwLock.AcquireReaderLock(Timeout.Infinite);
			try
			{
				return instance2Burden.Contains(instance);
			}
			finally
			{
				rwLock.ReleaseReaderLock();
			}
		}

		public void Release(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			rwLock.AcquireReaderLock(Timeout.Infinite);
			try
			{
				Burden burden = (Burden) instance2Burden[instance];

				if (burden == null)
					return;

				LockCookie cookie = rwLock.UpgradeToWriterLock(Timeout.Infinite);

				try
				{
					burden = (Burden) instance2Burden[instance];
					if (burden == null)
						return;

					instance2Burden.Remove(instance);

					burden.Release(this);
				}
				finally
				{
					rwLock.DowngradeFromWriterLock(ref cookie);
				}
			}
			finally
			{
				rwLock.ReleaseReaderLock();
			}
		}

		public void Dispose()
		{
			rwLock.AcquireWriterLock(Timeout.Infinite);
			try
			{
				int count = instance2Burden.Count;
				object[] instances = new object[count];
				instance2Burden.Keys.CopyTo(instances, 0);
				Burden[] burdens = new Burden[count];
				instance2Burden.Values.CopyTo(burdens, 0);

				for (int index = 0; index < count; ++index)
				{
					if (instance2Burden.Contains(instances[index]))
					{
						burdens[index].Release(this);
						instance2Burden.Remove(burdens[index]);
					}
				}
			}
			finally
			{
				rwLock.ReleaseWriterLock();
			}
		}
	}
}