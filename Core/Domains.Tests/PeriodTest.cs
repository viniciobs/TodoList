using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Domains.Tests
{
    [TestClass]
    public class PeriodTest
    {
        [TestMethod]
        public void CreateWithNullStartAndEndDateAndEnsureHasValuePropertyReturnsFalse()
        {
            var period = new Period(null, null);

            Assert.IsFalse(period.HasValue);
        }

        [TestMethod]
        [DataRow("2022-01-30", null)]
        [DataRow(null, "2022-01-30")]
        [DataRow("2022-01-30", "2022-01-31")]
        public void CreateWithNonNullParametersAndEnsureHasValuePropertyReturnsTrue(string startDate, string endDate)
        {
            DateTime? start = null;
            DateTime? end = null;

            if (!string.IsNullOrEmpty(startDate))
                start = DateTime.Parse(startDate);

            if (!string.IsNullOrEmpty(endDate))
                end = DateTime.Parse(endDate);

            var period = new Period(start, end);

            Assert.IsTrue(period.HasValue);
        }

        [TestMethod]
        [DataRow("2022-01-30T00:00:01", "2022-01-30T00:00:00")] // 1 second
        [DataRow("2022-01-31T00:00:00", "2022-01-30T00:00:00")] // 1 day
        public void TryCreatePeriodWithStartDateHigherThanEndDate_ThrowsRuleException(string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            Assert.ThrowsException<RuleException>(() => new Period(start, end));
        }

        [TestMethod]
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:00")] // Equal
        [DataRow("2022-01-31T00:00:00", "2022-01-31T00:00:01")] // 1 second higher
        [DataRow("2022-01-30T00:00:00", "2022-01-31T00:00:00")] // 1 day higher
        public void EnsureMehtodIsBetweenWithOnlyStartDate_OnlyHigherOrEqualDatesReturnTrue(string startDate, string dateToCompare)
        {
            var start = DateTime.Parse(startDate);
            var betweenDate = DateTime.Parse(dateToCompare);

            var period = new Period(start, null);

            Assert.IsTrue(period.IsBetween(betweenDate));
        }

        [TestMethod]
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:00")] // Equal
        [DataRow("2022-01-31T00:00:00", "2022-01-30T23:59:59")] // 1 second smaller
        [DataRow("2022-01-30T00:00:00", "2022-01-29T00:00:00")] // 1 day smaller
        public void EnsureMehtodIsBetweenWithOnlyEndDate_OnlySmallerOrEqualDatesReturnTrue(string endDate, string dateToCompare)
        {
            var end = DateTime.Parse(endDate);
            var betweenDate = DateTime.Parse(dateToCompare);

            var period = new Period(null, end);

            Assert.IsTrue(period.IsBetween(betweenDate));
        }

        [TestMethod]
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:00", "2022-01-30T00:00:00")] // Equal both
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:01", "2022-01-30T00:00:00")] // Equal startdate
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:01", "2022-01-30T00:00:01")] // Equal enddate
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:02", "2022-01-30T00:00:01")] // Higher than startdate and smaller than enddate
        public void EnsureMethodIsBetweenWithBothStartAndEndDatesReturnTrueForDatesBetweenThePeriod(string startDate, string endDate, string dateToCompare)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            var betweenDate = DateTime.Parse(dateToCompare);

            var period = new Period(start, end);

            Assert.IsTrue(period.IsBetween(betweenDate));
        }

        [TestMethod]
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:00", "2022-01-30T00:00:01")] // 1 second higher than both
        [DataRow("2022-01-30T00:00:01", "2022-01-30T00:00:05", "2022-01-30T00:00:00")] // 1 second smaller than startdate
        [DataRow("2022-01-30T00:00:00", "2022-01-30T00:00:05", "2022-01-30T00:00:06")] // 1 second higher than enddate
        public void EnsureMethodIsBetweenWithBothStartAndEndDatesReturnFalseForDatesOutsideThePeriod(string startDate, string endDate, string dateToCompare)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            var betweenDate = DateTime.Parse(dateToCompare);

            var period = new Period(start, end);

            Assert.IsFalse(period.IsBetween(betweenDate));
        }
    }
}