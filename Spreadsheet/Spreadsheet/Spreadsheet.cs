// Written by Joe Zachary for CS 3500, February 2017
//
// Feb 15, 2017 10:34 am:  Add restriction to specificiation of third overload of SetCellContents
//
// Modify for use with PS6 02/17/17
//           Add a new class SpreadsheetReadException
//           Add a new class SpreadsheetVersionException
//           Modify class comment for AbstractSpreadsheet
//           Add a new abstract property AbstractSpreadsheet.Changed.
//           Add a new method AbstractSpreadsheet.Save
//           Add a new method AbstractSpreadsheet.GetCellValue
//           Add a new abstract method AbstractSpreadsheet.SetContentsOfCell
//           Change the three AbstractSpreadsheet.SetCellContents methods to be protected


using Dependencies;
using Formulas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace SS
{

    // MODIFIED PARAGRAPHS 1-3 AND ADDED PARAGRAPH 4 FOR PS6
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of a regular expression (called IsValid below) and an infinite 
    /// number of named cells.
    /// 
    /// A string is a valid cell name if and only if (1) s consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits AND (2) the C#
    /// expression IsValid.IsMatch(s.ToUpper()) is true.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names, so long as they also
    /// are accepted by IsValid.  On the other hand, "Z", "X07", and "hello" are not valid cell 
    /// names, regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized by converting all letters to upper case before it is used by this 
    /// this spreadsheet.  For example, the Formula "x3+a5" should be normalize to "X3+A5" before 
    /// use.  Similarly, all cell names and Formulas that are returned or written to a file must also
    /// be normalized.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {



        /// <summary>
        /// The cell class is a abstraction in a spreadsheet, that contains 
        /// </summary>
        private class Cell
        {
            // cell name
            private string cellName { get; set; }

            /// <summary>
            /// Cell's content
            /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
            /// contents is an empty string, we say that the cell is empty. 
            /// </summary>
            public object content { get; set; }
            /// <summary>
            /// Cell's value
            /// If a cell's contents is a string, its value is that string.
            /// 
            /// If a cell's contents is a double, its value is that double.
            /// 
            /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
            /// The value of a Formula, of course, can depend on the values of variables.  The value 
            /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
            /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
            /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
            /// is a double, as specified in Formula.Evaluate.
            /// </summary>
            public object value { get; set; }

            /// <summary>
            /// constractor that create an empty cell
            /// </summary>
            /// <param name="name"></param>
            public Cell(string name)
            {
                this.cellName = name;
                this.content = "";
            }

            /// <summary>
            /// to string function for cell.
            /// If the cell contains a string, the string (without surrounding double quotes) is the output string.
            /// If the cell contains a double d, d.ToString() is the output string.
            /// If the cell contains a Formula f, f.ToString() with "=" prepended is the output string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if(content is Formula)
                {
                    return "=" + ((Formula)content).ToString();
                }
                else if(content is double)
                {
                    return ((double)content).ToString();
                }
                return (string)content;
            }
        }

        /// <summary>
        /// A DependencyGraph class is used to keep track of the relationships among spreadsheet cells
        /// </summary>
        private DependencyGraph dependencyGraph;

        /// <summary>
        /// A Dictionary containing all the cells in spreadsheet
        /// </summary>
        private Dictionary<string, Cell> cellSet;

        /// <summary>
        /// the C# expression IsValid.IsMatch(s.ToUpper()) is used in the program
        /// </summary>
        private Regex isValid { get; set; }


        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet() : this(new Regex(".*"))
        {
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        /// </summary>
        public Spreadsheet(Regex isValid)
        {
            Changed = false;
            this.isValid = isValid;
            dependencyGraph = new DependencyGraph();
            cellSet = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="isValid"></param>
        /// 
        public Spreadsheet(TextReader sourse, Regex isValid)
        {

            // Create the XmlSchemaSet class.  Anything with the namespace "urn:states-schema" will
            // be validated against states3.xsd.
            XmlSchemaSet sc = new XmlSchemaSet();

            // NOTE: To read states3.xsd this way, it must be stored in the same folder with the
            // executable.  To arrange this, I set the "Copy to Output Directory" propery of states3.xsd to
            // "Copy If Newer", which will copy states3.xsd as part of each build (if it has changed
            // since the last build).
            sc.Add(null, "Spreadsheet.xsd");

            // Configure validation.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += ValidationCallback;

            
            // Create the XmlReader object.
            using (XmlReader reader = XmlReader.Create(sourse, settings))
            {
                Regex oldIsValid = isValid;
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                // initialize spreadsheet
                                this.Changed = false;
                                this.dependencyGraph = new DependencyGraph();
                                this.cellSet = new Dictionary<string, Cell>();
                                try
                                {
                                    //If the exception is not thrown, this regex is referred to below as oldIsValid.
                                    oldIsValid = new Regex(reader.Value);
                                }
                                catch
                                {
                                    throw new SpreadsheetReadException("not a valid C# regular expression");
                                }
                                this.isValid = isValid;
                                break;

                            case "cell":

                                // Else if there is an invalid cell name or an invalid formula in the source, throws a 
                                // SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
                                // cell name validity.)
                                if (!isValidCellName(reader["name"], oldIsValid))
                                {
                                    throw new SpreadsheetReadException("oldIsValid not match");
                                }
                                // Else if there is an invalid cell name or an invalid formula in the source, throws a
                                // SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
                                // cell name validity.)
                                if (!isValidCellName(reader["name"], this.isValid))
                                {
                                    throw new SpreadsheetVersionException("newIsValid not match");
                                }
                                // If there is a duplicate cell name in the source, throws a SpreadsheetReadException.
                                // (Two cell names are duplicates if they are identical after being converted to upper case.)
                                if (cellSet.ContainsKey(reader["name"]))
                                {
                                    throw new SpreadsheetReadException("duplicate cell name");
                                }
                                SetContentsOfCell(reader["name"], reader["contents"]);
                                break;
                        }
                    }
                }
            }
        }

        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            throw new SpreadsheetReadException("saved spreadsheet could not be read because of a formatting problem");
        }

        // ADDED FOR PS6
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        // ADDED FOR PS6
        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            // if the spreadsheet is changed after created or saved, save the spreadsheet to XML file
            if(true)
            {
                using (XmlWriter writer = XmlWriter.Create(dest))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");

                    // The value of the IsValid attribute is IsValid.ToString()
                    writer.WriteAttributeString("IsValid", isValid.ToString());

                    // Add each none empty cell
                    foreach(string str in GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteAttributeString("name", str);
                        writer.WriteAttributeString("contents", cellSet[str].ToString());
                        writer.WriteFullEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                Changed = false;
            }
        }

        // ADDED FOR PS6
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(String name)
        {
            name = name.ToUpper();
            // If name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name, this.isValid))
            {
                throw new InvalidNameException();
            }

            // If cell does not exist add the cell as empty cell
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
                return "";
            }

            if(cellSet[name].content is Formula)
            {
                Formula f = (Formula)cellSet[name].content;
                foreach (string v in f.GetVariables())
                {
                    if (!cellSet.ContainsKey(v) || !(cellSet[v].value is double))
                    {
                        return new FormulaError();
                    }
                }
                // set cell value
                try{
                    return f.Evaluate(s => CellValueLookup(s));
                }
                catch
                {
                    return new FormulaError();
                }
                
            }
            else
            {
                return cellSet[name].content;
            }
        }



        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<String> GetNamesOfAllNonemptyCells()
        {
            foreach(string str in cellSet.Keys)
            {
                if (!cellSet[str].content.Equals(""))
                {
                    yield return str;
                }
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(String name)

        {
            
            if(name == null || !isValidCellName(name.ToUpper(), this.isValid))
            {
                throw new InvalidNameException();
            }

            name = name.ToUpper();
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            return cellSet[name].content;
        }

        // ADDED FOR PS6
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<String> SetContentsOfCell(String name, String content)
        {
            
            //If content is null, throws an ArgumentNullException.
            if(content == null)
            {
                throw new ArgumentNullException();
            }

            //if name is null or invalid, throws an InvalidNameException.
            if(name == null || !isValidCellName(name.ToUpper(), this.isValid))
            {
                throw new InvalidNameException();
            }
            name = name.ToUpper();
            //label the sheet have changed
            Changed = true;

            //if content parses as a double, the contents of the named cell becomes that double.
            double number = 0;
            if(double.TryParse(content, out number))
            {
                return SetCellContents(name, number);
            }

            //if content begins with the character '='
            if (content.Length>0 && content[0] == '=')
            {
                return SetCellContents(name, new Formula(content.Substring(1), s => s.ToUpper(), s => isValidCellName(s, this.isValid)));
            }

            //Otherwise, the contents of the named cell becomes content.
            return SetCellContents(name, content);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, double number)
        {
            // if name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name, this.isValid))
            {
                throw new InvalidNameException();
            }

            // add cell if cell is not exited
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            // remove dependees for new content
            dependencyGraph.ReplaceDependents(name,new HashSet<string>());
            
            // set cell content
            cellSet[name].content = number;
            
            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));

            return result;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, String text)
        {
            //If text is null, throws an ArgumentNullException.
            if (text == null)
            {
                throw new ArgumentNullException();
            }

            //if name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name, this.isValid))
            {
                throw new InvalidNameException();
            }

            //add the cell if the cell is not exist yet
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            // remove dependees for new content
            dependencyGraph.ReplaceDependents(name, new HashSet<string>());
            
            //set content and value
            cellSet[name].content = text;
            
            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));

            return result;
        }

        /// <summary>
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, Formula formula)
        {
            //if name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name, this.isValid))
            {
                throw new InvalidNameException();
            }

            //if name is null or invalid, throws an InvalidNameException.
            if (!cellSet.ContainsKey(name))
            {
                cellSet.Add(name, new Cell(name));
            }

            // remove dependees for new content
            dependencyGraph.ReplaceDependents(name, formula.GetVariables());

            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));

            // set cell's content and value
            cellSet[name].content = formula;
            cellSet[name].value = GetCellValue(name);
            return result;
        }




        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<String> GetDirectDependents(String name)
        {
            // If name is null, throws an ArgumentNullException
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            // if name isn't a valid cell name, throws an InvalidNameException.
            if (!isValidCellName(name, this.isValid))
            {
                throw new InvalidNameException();
            }
            
            ISet<string> result = new HashSet<string>();
            cellSet[name].value = GetCellValue(name);
            foreach (string str in dependencyGraph.GetDependees(name))
            {
                // update the value
                cellSet[str].value = GetCellValue(str);
                // unit dependees of one cell
                result.Add(str);
            }

            return result;
        }

        /// <summary>
        /// A lookup function that return double if cell's value is double, otherwise throw UndefinedVariableException
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        private double CellValueLookup(string var)
        {
            if (!cellSet.ContainsKey(var) || !(cellSet[var].value is double))
            {
                throw new UndefinedVariableException("variable not defined");
            }
            return (double)cellSet[var].value;
        }


        /// <summary>
        /// Helper method that return ture if cell name is valid, return false if cell name is not valid.
        /// Valid cell name consists of one or more letters, followed by a non-zero digit, followed by 
        /// zero or more digits.
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="isValidRegex"></param>
        /// <returns></returns>
        private bool isValidCellName(string cellName, Regex isValidRegex)
        {
            // if the cell name don't fit the basic contain, return false
            if(!Regex.IsMatch(cellName, "^[a-zA-z]{1,2}[1-9]{1}[0-9]*$"))
            {
                return false;
            }

            // if the isValid null, return true;
            if(isValidRegex == null)
            {
                return true;
            }
            return isValidRegex.IsMatch(cellName.ToUpper());
        }
    }
}
