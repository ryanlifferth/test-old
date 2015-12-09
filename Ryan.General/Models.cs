using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.General
{
    public class UserField
    {
        public int? UserFieldId { get; set; }
        public List<UserFieldTag> UserFieldTagList { get; set; }
        public List<UserFieldInterOperator> UserFieldInterOperatorList { get; set; }
    }

    public class UserFieldTag
    {

        public int? UserFieldTagId { get; set; }
        public int? UserFieldId { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string TagDisplayName { get; set; }
        public int Ordering { get; set; }
        public List<UserFieldTagOperator> TagOperatorList { get; set; }
        public string EndResult { get; set; }

    }

    public class UserFieldTagOperator
    {
        public int? UserFieldTagOperatorId { get; set; }
        public int? UserFieldTagId { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public string OperatorDisplayName { get; set; }
        public List<UserFieldTagOperatorTrait> UserFieldTagOperatorTraitList { get; set; }

        public UserFieldTagOperatorTrait GetUserFieldTagOperatorTrait(string attributeName)
        {
            return UserFieldTagOperatorTraitList.Where(a => a.TraitName == attributeName).FirstOrDefault();
        }
    }

    public class UserFieldTagOperatorTrait
    {
        public int? UserFieldTagOperatorTraitId { get; set; }
        public int? UserFieldTagOperatorId { get; set; }
        public int TraitId { get; set; }
        public string TraitName { get; set; }
        public string TraitDisplayName { get; set; }
        public string TraitDefaultValue { get; set; }
        public string UserFieldTagOperatorTraitValue { get; set; }

        public string GetUserFieldTagOperatorTraitValue()
        {
            return UserFieldTagOperatorTraitValue != null ? UserFieldTagOperatorTraitValue : TraitDefaultValue;
        }
    }

    public class UserFieldInterOperator
    {
        public int? UserFieldInterOperatorId { get; set; }
        public int? UserFieldId { get; set; }
        public int InterOperatorId { get; set; }
        public string InterOperatorCommonName { get; set; }
        public string InterOperatorDisplayName { get; set; }
        public int? LeftOperandReferenceId { get; set; }
        public object LeftOperandReference { get; set; }
        public bool LeftOperandReferencesSelf { get; set; }
        public int? RightOperandReferenceId { get; set; }
        public object RightOperandReference { get; set; }
        public bool RightOperandReferencesSelf { get; set; }
        public string EndResult { get; set; }
    }


}
