using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DescriptorSemaphore
    {
        private readonly IDictionary<int, SemaphoreSlim> _semaphoreByBucketNo;
        private readonly IDictionary<int, Guid> _tokenByBucketNo;

        public DescriptorSemaphore()
        {
            _semaphoreByBucketNo = new Dictionary<int, SemaphoreSlim>();
            _tokenByBucketNo = new Dictionary<int, Guid>();

            for (var i = 0; i <= 99; ++i)
            {
                _semaphoreByBucketNo[i] = new SemaphoreSlim(1, 1);
                _tokenByBucketNo[i] = Guid.Empty;
            }
        }

        public async Task<DescriptorSemaphoreRelease> WaitAsync(IDescriptor descriptor)
        {
            var bucketNo = DetermineBucketNo(descriptor);
            
            var semaphore = _semaphoreByBucketNo[bucketNo];
            await semaphore.WaitAsync();

            var token = Guid.NewGuid();
            _tokenByBucketNo[bucketNo] = token;

            return new DescriptorSemaphoreRelease(this, descriptor, token);
        }

        public void Release(IDescriptor descriptor, Guid token)
        {
            var bucketNo = DetermineBucketNo(descriptor);

            if (_tokenByBucketNo[bucketNo] == token)
            {
                _tokenByBucketNo[bucketNo] = Guid.Empty;
                _semaphoreByBucketNo[bucketNo].Release();
            }
        }

        private static int DetermineBucketNo(IDescriptor descriptor)
        {
            return Math.Abs(descriptor.GetHashCode() % 100);
        }
    }
}