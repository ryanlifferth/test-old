using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.General
{
    class Program
    {

        private static UserField _userField;

        static void Main(string[] args)
        {
            CreateNewFieldTag();

            //-- Test to make sure that the objects are the same
            Console.WriteLine("tag item 1 and tag item 2: {0}", _userField.UserFieldTagList.FirstOrDefault(t => t.TagName == "ContractDate").Equals(_userField.UserFieldInterOperatorList.FirstOrDefault(i => i.InterOperatorCommonName == "DateDifference").LeftOperandReference));
            Console.WriteLine("iteroperator item 1 and iteroperator item 2: {0}", _userField.UserFieldInterOperatorList.FirstOrDefault().Equals(_userField.UserFieldInterOperatorList.LastOrDefault().RightOperandReference));

            // So the app doesn't close down in debug mode
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        private static void CreateNewFieldTag()
        {
            _userField = new UserField()
            {
                UserFieldId = null,
                UserFieldTagList = new List<UserFieldTag>()
                {
                    new UserFieldTag()
                    {
                        UserFieldTagId = null,      //-- null for new object
                        UserFieldId = null,         //-- Parent object (container), null as well for new object
                        TagId = 19,                 //-- 19 - MLS Number
                        TagName = "MlsNumber",  
                        TagDisplayName = "MLS Number",
                        Ordering = 1,               //-- List order for items within this object
                        EndResult = "MLS#12345678", //-- Not sure what the origin is of this....
                        TagOperatorList = new List<UserFieldTagOperator>()
                        {
                            new UserFieldTagOperator 
                            {
                                UserFieldTagOperatorId = null,  //-- null for new object
                                UserFieldTagId = null,          //-- parent object (container), null as well for new object
                                OperatorId = 1,                 //-- 1 - TextAppend
                                OperatorName = "TextAppend",    
                                OperatorDisplayName = "Add text to value",
                                UserFieldTagOperatorTraitList = new List<UserFieldTagOperatorTrait>()
                                {
                                    new UserFieldTagOperatorTrait()
                                    {
                                        UserFieldTagOperatorTraitId = null,     //-- null for new object
                                        UserFieldTagOperatorId = null,          //-- parent object (container), null as well for new object
                                        TraitId = 5,                            //-- 5 = TextAppendBeforeData
                                        TraitName = "TextAppendBeforeData",
                                        TraitDisplayName = "Text Append Before Data",
                                        TraitDefaultValue = null,
                                        UserFieldTagOperatorTraitValue = "MLS#" //-- Value to append
                                    }
                                }
                            }
                        }
                    },
                    new UserFieldTag()
                    {
                        UserFieldTagId = null,      //-- null for new object
                        UserFieldId = null,         //-- Parent object (container), null as well for new object
                        TagId = 20,                 //-- 20 = Above Grade GLA
                        TagName = "AboveGradeGla",  
                        TagDisplayName = "Above Grade GLA",
                        Ordering = 1,               //-- List order for items within this object
                        EndResult = "2,050 sf",     //-- Not sure what the origin is of this....
                        TagOperatorList = new List<UserFieldTagOperator>()
                        {
                            new UserFieldTagOperator 
                            {
                                UserFieldTagOperatorId = null,  //-- null for new object
                                UserFieldTagId = null,          //-- parent object (container), null as well for new object
                                OperatorId = 8,                 //-- 8 = Numeric Format
                                OperatorName = "NumericFormat",
                                OperatorDisplayName = "Format number",
                                UserFieldTagOperatorTraitList = new List<UserFieldTagOperatorTrait>()
                                {
                                    new UserFieldTagOperatorTrait()
                                    {
                                        UserFieldTagOperatorTraitId = null,     //-- null for new object
                                        UserFieldTagOperatorId = null,          //-- parent object (container), null as well for new object
                                        TraitId = 2,                            //-- 2 = Comma
                                        TraitName = "Comma",
                                        TraitDisplayName = "Comma",
                                        TraitDefaultValue = "0",
                                        UserFieldTagOperatorTraitValue = null   //-- ???
                                    }
                                }
                            },
                            new UserFieldTagOperator 
                            {
                                UserFieldTagOperatorId = null,  //-- null for new object
                                UserFieldTagId = null,          //-- parent object (container), null as well for new object
                                OperatorId = 1,                 //-- 1 - TextAppend
                                OperatorName = "TextAppend",    
                                OperatorDisplayName = "Add text to value",
                                UserFieldTagOperatorTraitList = new List<UserFieldTagOperatorTrait>()
                                {
                                    new UserFieldTagOperatorTrait()
                                    {
                                        UserFieldTagOperatorTraitId = null,     //-- null for new object
                                        UserFieldTagOperatorId = null,          //-- parent object (container), null as well for new object
                                        TraitId = 6,                            //-- 6 = TextAppendAfterData
                                        TraitName = "TextAppendAfterData",
                                        TraitDisplayName = "Text Append After Data",
                                        TraitDefaultValue = null,
                                        UserFieldTagOperatorTraitValue = " sf"
                                    }
                                }
                            }
                        }
                    },
                    new UserFieldTag()
                    {
                        UserFieldTagId = null,      //-- null for new object
                        UserFieldId = null,         //-- Parent object (container), null as well for new object
                        TagId = 19,                 //-- 1 - Contract Date
                        TagName = "ContractDate",  
                        TagDisplayName = "Contract Date",
                        Ordering = 2,               //-- List order for items within this object
                        EndResult = "6/30/2015",    //-- Not sure what the origin is of this....
                        TagOperatorList = new List<UserFieldTagOperator>()
                        {
                            new UserFieldTagOperator 
                            {
                                UserFieldTagOperatorId = null,  //-- null for new object
                                UserFieldTagId = null,          //-- parent object (container), null as well for new object
                                OperatorId = 6,                 //-- 6 = DateFormat
                                OperatorName = "DateFormat",    
                                OperatorDisplayName = "Format date",
                                UserFieldTagOperatorTraitList = new List<UserFieldTagOperatorTrait>()
                                {
                                    new UserFieldTagOperatorTrait()
                                    {
                                        UserFieldTagOperatorTraitId = null,     //-- null for new object
                                        UserFieldTagOperatorId = null,          //-- parent object (container), null as well for new object
                                        TraitId = 4,                            //-- 4 = DateFormat
                                        TraitName = "DateFormat",
                                        TraitDisplayName = "Date Format",
                                        TraitDefaultValue = "MM/yy",            //-- Default date format value
                                        UserFieldTagOperatorTraitValue = "MM/dd/yyyy"   //-- Date format
                                    }
                                }
                            }
                        }
                    },
                    new UserFieldTag()
                    {
                        UserFieldTagId = null,      //-- null for new object
                        UserFieldId = null,         //-- Parent object (container), null as well for new object
                        TagId = 2,                  //-- 2 - Close Date
                        TagName = "CloseDate",  
                        TagDisplayName = "Close Date",
                        Ordering = 3,               //-- List order for items within this object
                        EndResult = "7/1/2015",     //-- Not sure what the origin is of this....
                        TagOperatorList = new List<UserFieldTagOperator>()
                        {
                            new UserFieldTagOperator 
                            {
                                UserFieldTagOperatorId = null,  //-- null for new object
                                UserFieldTagId = null,          //-- parent object (container), null as well for new object
                                OperatorId = 6,                 //-- 6 = DateFormat
                                OperatorName = "DateFormat",    
                                OperatorDisplayName = "Format date",
                                UserFieldTagOperatorTraitList = new List<UserFieldTagOperatorTrait>()
                                {
                                    new UserFieldTagOperatorTrait()
                                    {
                                        UserFieldTagOperatorTraitId = null,     //-- null for new object
                                        UserFieldTagOperatorId = null,          //-- parent object (container), null as well for new object
                                        TraitId = 4,                            //-- 4 = DateFormat
                                        TraitName = "DateFormat",
                                        TraitDisplayName = "Date Format",
                                        TraitDefaultValue = "MM/yy",            //-- Default date format value
                                        UserFieldTagOperatorTraitValue = "MM/dd/yyyy"   //-- Date format
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _userField.UserFieldInterOperatorList = new List<UserFieldInterOperator>()
            {
                new UserFieldInterOperator()
                {
                    UserFieldInterOperatorId = null,        //-- null for new object
                    UserFieldId = null,                     //-- parent object (container), null as well for new object
                    InterOperatorId = 1,                    //-- 1 = DateDifference
                    InterOperatorCommonName = "DateDifference",
                    InterOperatorDisplayName = "Date Difference",
                    LeftOperandReferenceId = null,          //-- null for new object
                    LeftOperandReference = _userField.UserFieldTagList.FirstOrDefault(t => t.TagName == "ContractDate"),    //-- Reference to the actual object
                    LeftOperandReferencesSelf = false,      //-- references an actual tag object
                    RightOperandReferenceId = null,         //-- null for new object
                    RightOperandReference = _userField.UserFieldTagList.FirstOrDefault(t => t.TagName == "CloseDate"),    //-- Reference to the actual object
                    RightOperandReferencesSelf = false,     //-- references an actual tag object
                    EndResult = "12"                        //-- int of difference (could be year, month, date hour, etc. depending on the date part)
                        
                }
            };
            _userField.UserFieldInterOperatorList.Add(
                new UserFieldInterOperator()
                {
                    UserFieldInterOperatorId = null,        //-- null for new object
                    UserFieldId = null,                     //-- parent object (container), null as well for new object
                    InterOperatorId = 2,                    //-- 2 = NumericAdd
                    InterOperatorCommonName = "NumericAdd",
                    InterOperatorDisplayName = "Numeric Add",
                    LeftOperandReferenceId = null,          //-- null for new object
                    LeftOperandReference = _userField.UserFieldTagList.FirstOrDefault(t => t.TagName == "ContractDate"),    //-- Reference to the actual object
                    LeftOperandReferencesSelf = false,      //-- references an actual tag object
                    RightOperandReferenceId = null,         //-- null for new object
                    RightOperandReference = _userField.UserFieldInterOperatorList.FirstOrDefault(i => i.InterOperatorCommonName == "DateDifference"),    //-- Reference to the actual object
                    RightOperandReferencesSelf = true,     //-- references an actual tag object
                    EndResult = "12"                        //-- int of difference (could be year, month, date hour, etc. depending on the date part)
                }
            );


        }



    }



}
