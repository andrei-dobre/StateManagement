using System;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DescriptorSemaphoreRelease
    {
        private readonly DescriptorSemaphore _semaphore;
        private readonly IDescriptor _descriptor;
        private readonly Guid _token;

        public DescriptorSemaphoreRelease(DescriptorSemaphore semaphore, IDescriptor descriptor, Guid token)
        {
            _semaphore = semaphore;
            _descriptor = descriptor;
            _token = token;
        }

        public void Release()
        {
            _semaphore.Release(_descriptor, _token);
        }
    }
}