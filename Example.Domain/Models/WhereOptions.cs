using Example.Domain.Enums;
using Example.Domain.Exceptions;

namespace Example.Domain.Models
{
    public class WhereOptions
    {
        public string? FilterBy { get; set; }
        public string? Value { get; set; }
        public WhereOperator Operation { get; set; } = WhereOperator.Equal;

        public WhereOptions() { }

        public WhereOptions(string? filterBy, string? value, WhereOperator operation)
        {
            FilterBy = filterBy;
            Value = value;
            Operation = operation;
        }

        public WhereOptions(string condition)
        {
            ToObject(condition);
        }

        private void ToObject(string condition)
        {
            // Name eq Raquel
            var split = condition.Split(' ');
            if (split.Length < 3)
            {
                var args = new[] { "Attribute", "Operation", "Value match", "Name Eq Ismael" };
                throw new BadRequestException($"Filter not valid...", args);
            }

            this.FilterBy = split[0];
            this.Operation = GetOperator(split[1]);
            this.Value = condition.Split($"{split[0]} {split[1]} ")[1];
        }

        private static WhereOperator GetOperator(string condition)
        {
            return condition switch
            {
                "eq" or "Eq" or "EQ" => WhereOperator.Equal,
                "ne" or "Ne" or "NE" => WhereOperator.NotEqual,
                "gt" or "Gt" or "GT" => WhereOperator.GreaterThan,
                "ge" or "Ge" or "GE" => WhereOperator.GreaterThanOrEqual,
                "lt" or "Lt" or "LT" => WhereOperator.LessThan,
                "le" or "Le" or "LE" => WhereOperator.LessThanOrEqual,
                "lk" or "Lk" or "LK" => WhereOperator.Like,
                _ => WhereOperator.Equal,
            };
        }
    }
}
