using DigitalDatasheet.Data;
using DigitalDatasheet.Models;
using DigitalDatasheetContextLib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace DigitalDatasheet.Documents
{
	public class HardCopy
	{
		Word.Application WordApp { get; set; }
		Word.Document WordDoc { get; set; }
		string WorkOrderNumber { get; set; }
		string TestCondition { get; set; }
		string TestConditionAbr { get; set; }
		string TestPerformedOn { get; set; }
		string DateReceivedYear { get; set; }
		string ErrorLogFilePath { get; } = $@"\\ptlsrvr4\PTLOffice\Digital Datasheet Forms\Digital Datasheet Error Log\{DateTime.Now:D}.txt";
		StreamWriter sw;

		public HardCopy(string workOrderNumber, string testCondition, string testPerformedOn)
		{
			WorkOrderNumber = workOrderNumber;
			TestCondition = testCondition;
			TestConditionAbr = testCondition == "As Received" ? "AR" : "TS";
			TestPerformedOn = testPerformedOn;

			try
			{
				WordApp = new Word.Application();
				//WordApp.Visible = true;
				string path = testCondition == "As Received" ? @"\\ptlsrvr4\PTLOffice\Digital Datasheet Forms\AR_datasheet_template.docx" : @"\\ptlsrvr4\PTLOffice\Digital Datasheet Forms\TS_datasheet_template.docx";
				//string path = testCondition == "As Received" ? @"C:\Users\Nicholas\Documents\PTL\AR_datasheet_template.docx" : @"C:\Users\Nicholas\Documents\PTL\TS_datasheet_template.docx";
				WordDoc = WordApp.Documents.Open(path, true, true);
			}
			catch (Exception err)
			{
				sw = new StreamWriter(ErrorLogFilePath, true);
				sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nHardCopy constructor -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				sw.Close();

				if (WordDoc != null)
					WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
				if (WordApp != null)
					WordApp.Quit();
				throw;
			}
		}
		public void SetHeaderInfo(List<string> headerInfo)
		{
			try
			{
				//string dateTested = headerInfo[0].ToString() == "" ? "" : ((DateTime)headerInfo[0]).ToShortDateString();
				WordDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Find.Execute("<date tested>", true, true, false, false, false, true, 1, false, headerInfo[0], 2, false, false, false, false);
				WordDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Find.Execute("<tested by>", true, true, false, false, false, true, 1, false, headerInfo[1], 2, false, false, false, false);
				WordDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Find.Execute("<check by>", true, true, false, false, false, true, 1, false, headerInfo[2], 2, false, false, false, false);
			}
			catch (Exception err)
			{
				sw = new StreamWriter(ErrorLogFilePath, true);
				sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nHardCopy Set_Header_Info -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				sw.Close();

				if (WordDoc != null)
					WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
				if (WordApp != null)
					WordApp.Quit();
				throw;
			}
		}
		/// <summary>
		/// insert standard job information to test report (work order number, test condition, part number, date code)
		/// </summary>
		public void SetJobInfo(Form jobInfo)
		{
			try
			{
				int testPerformedOnNum = 0;
				switch (TestPerformedOn)
				{
					case "Coupons":
						testPerformedOnNum = 1;
						break;
					case "BareBoards":
						testPerformedOnNum = 2;
						break;
					case "CustomerMounts":
						testPerformedOnNum = 3;
						break;
					case "AssembledBoards":
						testPerformedOnNum = 4;
						break;
					case "Class2Assessment":
						testPerformedOnNum = 5;
						break;
				}
				for (int i = 1; i <= 5; i++)
				{
					if (testPerformedOnNum == i)
						WordApp.Selection.Find.Execute($"<{i}>", true, true, false, false, false, true, 1, false, "X", 2, false, false, false, false);
					else
						WordApp.Selection.Find.Execute($"<{i}>", true, true, false, false, false, true, 1, false, "_", 2, false, false, false, false);
					if (testPerformedOnNum == 3)
						WordApp.Selection.Find.Execute("<#>", true, true, false, false, false, true, 1, false, jobInfo.CustomerMountQty, 2, false, false, false, false);
					else
						WordApp.Selection.Find.Execute("<#>", true, true, false, false, false, true, 1, false, "_", 2, false, false, false, false);
				}

				WordApp.Selection.Find.Execute("<wo number>", true, true, false, false, false, true, 1, false, WorkOrderNumber, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<part number>", true, true, false, false, false, true, 1, false, jobInfo.PartNumber, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<lot number>", true, true, false, false, false, true, 1, false, jobInfo.LotNumber, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<customer>", true, true, false, false, false, true, 1, false, jobInfo.Customer, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<date code>", true, true, false, false, false, true, 1, false, jobInfo.DateCode, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<specification 1>", true, true, false, false, false, true, 1, false, jobInfo.Specification1, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<specification 2>", true, true, false, false, false, true, 1, false, jobInfo.Specification2, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<board type>", true, true, false, false, false, true, 1, false, jobInfo.BoardType, 2, false, false, false, false);
				WordApp.Selection.Find.Execute("<test procedure>", true, true, false, false, false, true, 1, false, jobInfo.TestProcedure, 2, false, false, false, false);

				//string drawingProvided = jobInfo.DrawingProvided == 1 ? "YES" : "NO";
				WordApp.Selection.Find.Execute("<drawing provided>", true, true, false, false, false, true, 1, false, jobInfo.DrawingProvided == 1 ? "YES" : "NO", 2, false, false, false, false);

				WordApp.Selection.Find.Execute("<evaluated by>", true, true, false, false, false, true, 1, false, jobInfo.EvaluatedBy, 2, false, false, false, false);
				//string dateEval = jobInfo[14].ToString() == "" ? "" : ((DateTime)jobInfo[14]).ToShortDateString();
				WordApp.Selection.Find.Execute("<date eval>", true, true, false, false, false, true, 1, false, jobInfo.DateEvaluated.HasValue ? (jobInfo.DateTested as DateTime?).Value.ToShortDateString() : string.Empty, 2, false, false, false, false);

				if (TestConditionAbr == "TS")
				{
					WordApp.Selection.Find.Execute("<time in>", true, true, false, false, false, true, 1, false, jobInfo.BakeTimeIn, 2, false, false, false, false);
					WordApp.Selection.Find.Execute("<time out>", true, true, false, false, false, true, 1, false, jobInfo.BakeTimeOut, 2, false, false, false, false);
					WordApp.Selection.Find.Execute("<hrs>", true, true, false, false, false, true, 1, false, jobInfo.TotalTime, 2, false, false, false, false);
					WordApp.Selection.Find.Execute("<temp>", true, true, false, false, false, true, 1, false, jobInfo.TestTemp, 2, false, false, false, false);
					WordApp.Selection.Find.Execute("<floats>", true, true, false, false, false, true, 1, false, $"{jobInfo.SolderFloats}X", 2, false, false, false, false);
				}
			}
			catch (Exception err)
			{
				sw = new StreamWriter(ErrorLogFilePath, true);
				sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nHardCopy Set_Job_Info -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				sw.Close();

				if (WordDoc != null)
					WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
				if (WordApp != null)
					WordApp.Quit();
				throw;
			}
		}
		public async Task SetData()
		{
			try
			{
				await using (var db = new DigitalDatasheetContext())
				{
					// check locations for multiple coupon set condition
					var locations = db.JobDataTable
						.Where(data => data.WorkOrderNumber.Equals(WorkOrderNumber) && data.TestCondition.Equals(TestCondition) && data.TestPerformedOn.Equals(TestPerformedOn))
						.Select(r => r.Location)
						.Distinct();
					if (locations.Count() > 1)
						WordApp.Selection.Find.Execute("<cpn set>", true, true, false, false, false, true, 1, false, "Multiple Coupon Set", 2, false, false, false, false);
					else
						WordApp.Selection.Find.Execute("<cpn set>", true, true, false, false, false, true, 1, false, "", 2, false, false, false, false);

					var records = await db.JobDataTable
						.Where(data => data.WorkOrderNumber.Equals(WorkOrderNumber) && data.TestCondition.Equals(TestCondition) && data.TestPerformedOn.Equals(TestPerformedOn))
						.OrderBy(data => data.StructureOrder)
						.ThenBy(data => data.Row)
						.ToListAsync();

					var dataTable = WordDoc.Tables[2];
					// create inital set table based on number of structures there are
					// base table will have one row for each structure and below each structure, one row for measurements

					var structureList = records
						.OrderBy(r => r.StructureOrder)
						.Select(r => new { r.StructureTitle })
						.Distinct()
						.ToList();

					for (int i = 1; i < structureList.Count; i++)
					{
						dataTable.Rows.Add(dataTable.Rows[dataTable.Rows.Count]);
						dataTable.Rows.Add(dataTable.Rows[dataTable.Rows.Count]);
						dataTable.Cell(dataTable.Rows.Count - 1, 1).Merge(dataTable.Cell(dataTable.Rows.Count - 1, 20));
					}
					//data_table.Cell(1, 1).Range.Text = structure_list[0];

					// set row for structure titles to same format (center vertical and horizontal alignment and font)
					for (int i = 3; i < dataTable.Rows.Count; i += 2)
					{
						dataTable.Cell(i, 1).Range.Font = dataTable.Cell(1, 1).Range.Font;
						dataTable.Cell(i, 1).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
					}

					int endRow = dataTable.Rows.Count;
					// loop through each structure starting from the end
					for (int i = structureList.Count - 1; i >= 0; i--)
					{
						var recordSet = records
							.Where(r => r.StructureTitle == structureList[i].StructureTitle)
							.OrderBy(r => r.Row)
							.ToList();
						// set last structure title row to correct structure
						dataTable.Cell(endRow - 1, 1).Range.Text = structureList[i].StructureTitle;
						// loop through each location/serial number combo for each structure starting from the end
						for (int j = recordSet.Count - 1; j >= 0; j--)
						{
							// if serial number has associated location add location and set font color to red
							if (string.IsNullOrEmpty(recordSet[j].Location))
							{
								// set first column of row to serial number
								dataTable.Cell(endRow, 1).Range.Text = recordSet[j].SerialNumber;
								dataTable.Cell(endRow, 1).Range.Font.Color = Word.WdColor.wdColorBlack;
								dataTable.Cell(endRow, 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;
							}
							else
							{
								// set first column of row to location/serial number
								dataTable.Cell(endRow, 1).Range.Text = $"loc {recordSet[j].Location}\n{recordSet[j].SerialNumber}";
								dataTable.Cell(endRow, 1).Range.Font.Color = Word.WdColor.wdColorBlack;
								dataTable.Cell(endRow, 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;
								// set location font color to red
								int wordCount = dataTable.Cell(endRow, 1).Range.Words.Count;
								for (int k = 1; k < wordCount; k++)
								{
									Word.Range word = dataTable.Cell(endRow, 1).Range.Words[k];
									if (word.Text.StartsWith("loc"))
									{
										while (dataTable.Cell(endRow, 1).Range.Words[k].Text != "\r" && k != wordCount)
											dataTable.Cell(endRow, 1).Range.Words[k++].Font.Color = Word.WdColor.wdColorRed;
									}
								}
							}

							// set remaining columns of row to measurements and observations
							List<string> dataRow = new List<string>
							{
								recordSet[j].HoleCuPlating,
								recordSet[j].ExternalConductor,
								recordSet[j].SurfaceCladCu,
								recordSet[j].WrapCu,
								recordSet[j].CapCu,
								recordSet[j].InternalCladCu,
								recordSet[j].MinEtchback,
								recordSet[j].MaxEtchback,
								recordSet[j].InternalAnnularRing,
								recordSet[j].ExternalAnnularRing,
								recordSet[j].Dielectric,
								recordSet[j].Wicking,
								recordSet[j].InnerlayerSeparation,
								recordSet[j].PlatingCrack,
								recordSet[j].PlatingVoid,
								recordSet[j].DelamBlisters,
								recordSet[j].LaminateVoidCrack,
								recordSet[j].AcceptReject
							};
							// loop through data row setting columns to each value
							int column = 2;
							foreach (string data in dataRow)
							{
								if (string.IsNullOrEmpty(data))
								{
									column++;
									continue;
								}
								if (column == 19)
									column++;
								// check if measurement starts with an R (if it does, set the cell background color to yellow and remove the R)
								if (column <= 13)
								{
									string newData = data;

									if (data.StartsWith("R"))
									{
										if (data.Contains("^"))
										{
											string[] splitData = data.Split('^');
											newData = $"{splitData[1]}\n{splitData[0]}";
										}
										newData = newData.Remove(newData.IndexOf('R'), 1);
										dataTable.Cell(endRow, column).Range.Text = newData;
										dataTable.Cell(endRow, column).Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;
										dataTable.Cell(endRow, 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;
									}
									else if (data.Contains("^"))
									{
										string[] splitData = data.Split('^');
										newData = $"{splitData[1]}\n{splitData[0]}";

										dataTable.Cell(endRow, column).Range.Text = $"{newData}";
										dataTable.Cell(endRow, column).Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;
									}
									else
									{
										dataTable.Cell(endRow, column).Range.Text = data;
										if (data == "N/A")
											dataTable.Cell(endRow, column).Shading.BackgroundPatternColor = Word.WdColor.wdColorGray10;
										else
											dataTable.Cell(endRow, column).Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;
									}
								}
								else
								{
									dataTable.Cell(endRow, column).Range.Text = data;
									if (data == "R")
									{
										dataTable.Cell(endRow, column).Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;
										dataTable.Cell(endRow, 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;
									}
									else
										dataTable.Cell(endRow, column).Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;
								}
								column++;
							}

							// if this is the final row of the data list, do not add new row to word document
							if (j != 0)
								dataTable.Rows.Add(dataTable.Rows[endRow]);
						}
						endRow -= 2;
					}
				}
			}
			catch (Exception err)
			{
				sw = new StreamWriter(ErrorLogFilePath, true);
				sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nHardCopy Set_Data -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				sw.Close();

				if (WordDoc != null)
					WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
				if (WordApp != null)
					WordApp.Quit();
				throw;
			}
		}
		public void SetRequirements(List<string> requirements)
		{
			try
			{
				var requirementsTable = WordDoc.Tables[3];
				int column = 2;
				foreach (string requirement in requirements)
				{
					//if (requirementsTable.Cell(1, column).Range.Text.StartsWith("N/A"))
					//{
					//    column++;
					//    continue;
					//}
					if (string.IsNullOrEmpty(requirement))
						column++;
					else if (requirement == "collapsed")
						requirementsTable.Cell(1, column - 1).Merge(requirementsTable.Cell(1, column));
					else if (requirement.Contains("\n") &&
							!requirement.ToLower().StartsWith("layer") &&
							!requirement.ToLower().StartsWith("x:") &&
							!requirement.ToLower().StartsWith("negative") &&
							!requirement.ToLower().StartsWith("smear"))
					{
						// separate each requirement
						string[] reqSplit = requirement.Split('\n');
						//List<string> reqList = new List<string>();
						//string formatReq = "";
						//foreach (string individualReq in reqSplit)
						//{
						//	string req = individualReq.Trim();
						//	formatReq += req;
						//}
						// split the current cell setting number of rows equal to numnber of requirements
						requirementsTable.Cell(1, column).Split(reqSplit.Length, 1);
						int row = 1;
						foreach (string individualReq in reqSplit)
						{
							string req = individualReq.Trim();
							if (req.Contains(" ("))
							{
								req = req.Replace(" (", "\n(");
							}
							requirementsTable.Cell(row++, column).Range.Text = req;
						}
						column++;
					}
                    else
                    {
						string req = requirement;
						if (req.Contains(" ("))
						{
							req = req.Replace(" (", "\n(");
						}
						requirementsTable.Cell(1, column++).Range.Text = req;
                    }
				}
			}
			catch (Exception err)
			{
				sw = new StreamWriter(ErrorLogFilePath, true);
				sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nHardCopy Set_Requirements -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				sw.Close();

				if (WordDoc != null)
					WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
				if (WordApp != null)
					WordApp.Quit();
				throw;
			}
		}
		public void SetRemarks(List<(string, bool)> remarksStatus)
		{
			try
			{
				var remarksTable = WordDoc.Tables[4];
				for (int i = 1; i < remarksStatus.Count; i++)
					remarksTable.Rows.Add(remarksTable.Rows[1]);

				int row = 1;
				foreach ((string remark, bool status) in remarksStatus)
				{
					Word.Cell cell = remarksTable.Cell(row++, 1);
					if (status)
						cell.Range.Font.Shading.BackgroundPatternColor = Word.WdColor.wdColorYellow;
					else
						cell.Range.Font.Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;
					cell.Range.Text = remark;
				}
			}
			catch (Exception err)
			{
				sw = new StreamWriter(ErrorLogFilePath, true);
				sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nHardCopy Set_Remarks -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				sw.Close();

				if (WordDoc != null)
					WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
				if (WordApp != null)
					WordApp.Quit();
				throw;
			}
		}
		public async Task SaveAndClose(string customer)
		{
			try
			{
				//app.Visible = true;
				// get path to directory of current work order number and determine if it exists
				// job path = \\ptlsrvr4 \ year received \ first letter of customer (i.e. _A to M) \ customer name \ work order number
				string letterFolder, fullDir, filePath;
				DateReceivedYear = await new AccessDb().GetJobYear(WorkOrderNumber);
				if (string.IsNullOrEmpty(DateReceivedYear))
				{
					MessageBox.Show("Invalid date received for current job. Please check and make sure job information is correct.", "Job Date Error", MessageBoxButton.OK, MessageBoxImage.Error);
					WordApp.Visible = true;
					return;
				}

				//JobData job_data = new JobData();
				//date_received_year = job_data.Get_Date_Received_Year(wo_number, wo_dash);

				Regex letterFolderRegex = new Regex(@"^[a-mA-M0-9]");
				letterFolder = letterFolderRegex.IsMatch(customer) ? "_A to M" : "_N to Z";

				fullDir = $@"\\ptlsrvr4\j\{DateReceivedYear}\{letterFolder}\{customer}\{WorkOrderNumber}";
				//fullDir = $@"C:\Users\Nicholas\Documents\PTL";
				//DirectoryInfo job_path = new DirectoryInfo(fullDir);
				if (!Directory.Exists(fullDir))
				{
					MessageBox.Show("The current job folder does not exist. Please check and make sure this work order number is correct and has been logged.", "No Job Folder", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					WordApp.Visible = true;
					return;
				}
				filePath = $@"{fullDir}\{WorkOrderNumber}_{TestConditionAbr}_{TestPerformedOn}_Metallographic_Examination_datasheet_raw.docx";
				// determine if file already exists and ask before overwriting
				if (File.Exists(filePath))
				{
					if (MessageBox.Show("Datasheet hard copy already exists. Saving current file will override existing datasheet. Would you like to continue and save anyway?", "File Already Exists", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
					{
						WordApp.Visible = true;
						return;
					}
				}
				WordDoc.SaveAs2(filePath);
				if (MessageBox.Show("Datasheet hard copy has been created and saved in job folder. Would you like to open it now?", "Open Datasheet", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
                    try
                    {
                        // open directoruy containing data report
                        Process.Start(fullDir);
                    }
                    catch (Exception) {}
					// open data report
					WordApp.Visible = true;
				}
				else
				{
					WordDoc.Close();
					WordApp.Quit();
				}
			}
			catch (Exception err)
			{
				sw = new StreamWriter(ErrorLogFilePath, true);
				sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nHardCopy Save_And_Close -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				sw.Close();

				if (WordApp != null)
					WordApp.Visible = true;

				//if (WordDoc != null)
				//    WordDoc.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
				//if (WordApp != null)
				//    WordApp.Quit();
				throw;
			}
		}
	}
}