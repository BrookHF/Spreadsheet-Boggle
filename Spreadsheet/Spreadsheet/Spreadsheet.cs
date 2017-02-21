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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        /// </summary>
        public Spreadsheet(Regex isValid)
        {
            this.isValid = isValid;
            dependencyGraph = new DependencyGraph();
            cellSet = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Zero argument constructor for spreadsheet
        /// </summary>
        public Spreadsheet() : this(new Regex(".*"))
        {
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
            // If name is null or invalid, throws an InvalidNameException.
            if (name == null || !isValidCellName(name))
            {
                throw new InvalidNameException();
            }

            return null;
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
            if(name == null || !isValidCellName(name))
            {
                throw new InvalidNameException();
            }
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
            if(name == null || isValidCellName(name))
            {
                throw new InvalidNameException();
            }

            //if content parses as a double, the contents of the named cell becomes that double.
            double number = 0;
            if(double.TryParse(content, out number))
            {
                return SetCellContents(name, number);
            }

            //if content begins with the character '='
            if (content[0] == '=')
            {
                return SetCellContents(name, new Formula(content.Substring(1), s => s.ToUpper(), s => isValidCellName(s)));
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
            if (name == null || !isValidCellName(name))
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

            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));

            // set cell content and value
            cellSet[name].content = number;

            // set cell value
            cellSet[name].value = number;

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
            if (name == null || !isValidCellName(name))
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

            //using result set to put name plus the names of all other cells whose value depends
            ISet<string> result = new HashSet<string>(GetCellsToRecalculate(name));

            //set content and value
            cellSet[name].content = text;

            // set cell value
            cellSet[name].value = text;

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
            if (name == null || !isValidCellName(name))
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
            cellSet[name].content = formula;

            // set cell value
            cellSet[name].value = formula.Evaluate(s => cellSet[s].value);

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
            if (!isValidCellName(name))
            {
                throw new InvalidNameException();
            }

            // unit dependees and dependents of one cell
            ISet<string> result = new HashSet<string>(dependencyGraph.GetDependees(name));

            return result;
        }

        /// <summary>
        /// Helper method that return ture if cell name is valid, return false if cell name is not valid.
        /// Valid cell name consists of one or more letters, followed by a non-zero digit, followed by 
        /// zero or more digits.
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        private bool isValidCellName(string cellName)
        {
            return Regex.IsMatch(cellName, "^[a-zA-z]{1,2}[1-9]{1}[0-9]*$") && isValid.IsMatch(cellName.ToUpper());
        }



    }
}
