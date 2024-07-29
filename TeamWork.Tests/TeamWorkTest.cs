using System;
using System.Collections.Generic;
using Xunit;
using TeamWork;

namespace TeamWork.Tests
{
    public class WorkerTests
    {
        [Fact]
        public void Worker_Initialization_ShouldSetCorrectCapacity()
        {
            decimal speed = 2m; // 1 image per 2 minutes
            Worker worker = new Worker(speed);

            Assert.Equal(0.5m, worker.GetCapacity());
        }

        [Fact]
        public void Worker_CalculateProcessedImages_ShouldReturnCorrectCount()
        {
            decimal speed = 2m;
            Worker worker = new Worker(speed);
            decimal time = 10m;

            int processedImages = worker.CalculateProcessedImages(time);

            Assert.Equal(5, processedImages);
            Assert.Equal(0, worker.PictureReadiness);
        }
    }

    public class BrigadeTests
    {
        [Fact]
        public void CalculateBrigadeDataForTwoWorkers()
        {
            List<Worker> workers = new List<Worker>
            {
                new Worker(2m), // 0.5 images per minute
                new Worker(3m)  // 0.333... images per minute
            };

            Brigade brigade = new Brigade(workers);
            brigade.CalculateImagesProcessing(10);

            Assert.Equal(0.833m, brigade.CalculateBrigadeCapacity(), 3);
            Assert.Equal(10, brigade.GetProcessedImages());
            Assert.True(brigade.GetTime() <= brigade.GetApproximateTime());
            Assert.Equal(12, brigade.GetApproximateTime(), 1);
            Assert.Equal(12, brigade.GetTime());
        }

        [Fact]
        public void CalculateBrigadeDataForSingleWorker()
        {
            List<Worker> workers = new List<Worker>
            {
                new Worker(2m)
            };

            Brigade brigade = new Brigade(workers);
            brigade.CalculateImagesProcessing(10);

            Assert.Equal(0.5m, brigade.CalculateBrigadeCapacity());
            Assert.Equal(10, brigade.GetProcessedImages());
            Assert.Equal(20, brigade.GetTime());
            Assert.Equal(20, brigade.GetApproximateTime());
        }

        [Theory]
        [InlineData(2, 3, 4)]
        [InlineData(5, 7, 13)]
        [InlineData(7, 13, 19)]
        [InlineData(23, 2, 10)]
        [InlineData(100, 0.5, 0.25)]
        public void CalculateBrigadeDataForThreeWorkers(decimal speed1, decimal speed2, decimal speed3)
        {
            List<Worker> workers = new List<Worker>
            {
                new Worker(speed1),
                new Worker(speed2),
                new Worker(speed3)
            };

            Brigade brigade = new Brigade(workers);
            brigade.CalculateImagesProcessing(1000);

            Assert.True(brigade.GetProcessedImages() == 1000 || brigade.GetProcessedImages() == 1001);
            Assert.True(brigade.GetTime() >= brigade.GetApproximateTime());
            Assert.True(brigade.GetUnfinishedImages() >= 0);
        }
    }
}
