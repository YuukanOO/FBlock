using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.IO;
using FBlock.Core;

namespace FBlock.UnitTests.IO
{
    [TestClass]
    public class CSVReader
    {
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestWrongFilePath()
        {
            Job<string, DataTable> job = new Job<string, DataTable>();
            job.StartAndEnd(new Components.IO.CSVReader());
                        
            DataTable table = job.Process("apath/that/doesnot/exist.csv");
        }

        [TestMethod]
        public void TestReadNoHeader()
        {
            Job<string, DataTable> job = new Job<string, DataTable>();
            job.StartAndEnd(new Components.IO.CSVReader());
            
            DataTable table = job.Process(TestConstants.CSV_SAMPLE_PATH);

            Assert.AreEqual(18, table.Columns.Count);
            Assert.AreEqual(60, table.Rows.Count);

            Assert.AreEqual("county", table.Rows[0][2]);
            Assert.AreEqual("433512", table.Rows[9][0]);
            Assert.AreEqual("-81.704613", table.Rows[9][14]);
        }

        [TestMethod]
        public void TestReadHeaders()
        {
            Job<string, DataTable> job = new Job<string, DataTable>();
            job.StartAndEnd(new Components.IO.CSVReader(true));

            DataTable table = job.Process(TestConstants.CSV_SAMPLE_PATH);

            Assert.AreEqual(18, table.Columns.Count);
            Assert.AreEqual(59, table.Rows.Count);

            Assert.AreEqual("policyID", table.Columns[0].ColumnName);
            Assert.AreEqual("point_longitude", table.Columns[14].ColumnName);

            Assert.AreEqual("433512", table.Rows[8]["policyID"]);
            Assert.AreEqual("-81.704613", table.Rows[8]["point_longitude"]);
        }
    }
}
