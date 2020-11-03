using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using Microsoft.VisualBasic.FileIO;

namespace SoftwareDev_Test
{
    class ReadFromText
    {
        /// <summary>
        /// Read CSV data from text file and convert to a datatble. Only one set of data/table is supported
        /// </summary>
        /// <param name="fileFullPathName"></param>
        /// <returns></returns>
        public DataTable GetCSVAsTable(string fileFullPathName)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(fileFullPathName))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn dc = new DataColumn(column)
                        {
                            AllowDBNull = true
                        };
                        csvData.Columns.Add(dc);
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }

                    csvData.AcceptChanges();

                }
                return csvData;
            }
            catch (Exception ex)
            {
                throw new Exception("ReadFromText.GetCSVAsTable() :" + ex.Message);
            }

            finally
            {
                csvData.Dispose();
            }

        }
    }
}
