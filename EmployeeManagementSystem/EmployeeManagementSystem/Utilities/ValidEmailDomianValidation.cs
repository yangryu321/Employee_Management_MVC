namespace EmployeeManagementSystem.Utilities
{
    public class ValidEmailDomianValidation : ValidationAttribute
    {
        private readonly string alloweddomain;

        public ValidEmailDomianValidation(string alloweddomain)
        {
            this.alloweddomain = alloweddomain;
        }

        public override bool IsValid(object? value)
        {
            string[] strs = value.ToString().Split('@');

            if(strs[1].ToUpper() == alloweddomain.ToUpper())
            {
                return true;
            }
            return false;
        }
    }
}
