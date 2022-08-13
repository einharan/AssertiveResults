using RegularExpression = System.Text.RegularExpressions.Regex;
using AssertiveResults.Assertions.RegularExpressions.Clauses;
using AssertiveResults.Errors;

namespace AssertiveResults.Assertions.RegularExpressions
{
    internal sealed class Regex : IRegex, IMatch, IResult
    {
        private readonly Context _context;
        private string _input;

        internal Regex(Context context)
        {
            this._context = context;
            Contains = new Contains(this);
            Format = new Format(this);
        }

        public IContains Contains { get; internal set; }
        public IFormat Format { get; internal set; }

        public IMatch Validate(string input)
        {
            _input = input;
            return this;
        }

        public IResult Matches(string pattern)
        {
            return Assert(pattern,
                ErrorCode.Assertion.RegularExpression,
                string.Format(ErrorDescription.StringNotMatchesRegularExpression, pattern));
        }

        public IResult MatchesIllegal(string pattern)
        {
            return Assert(pattern,
                ErrorCode.Assertion.RegularExpression,
                string.Format(ErrorDescription.StringMatchesIllegalRegularExpression, pattern),
                illegal: true);
        }

        public IResult Length(int min, int max)
        {
            return Assert(RegexPattern.Length(min, max),
                ErrorCode.Assertion.RegularExpression,
                string.Format(ErrorDescription.StringInvalidLength, min, max));
        }

        public IResult MinLength(int min)
        {
            return Assert(RegexPattern.MinLength(min),
                ErrorCode.Assertion.RegularExpression,
                string.Format(ErrorDescription.StringTooShort, min));
        }

        public IResult MaxLength(int max)
        {
            return Assert(RegexPattern.MaxLength(max),
                ErrorCode.Assertion.RegularExpression,
                string.Format(ErrorDescription.StringTooLong, max));
        }

        public IMatch WithError(Error error)
        {
            _context.WithError(error);
            return this;
        }

        internal IResult Assert(
            string pattern,
            string errorCode,
            string errorMessages,
            bool illegal = false)
        {
            var regex = new RegularExpression(pattern);
            var isMatch = regex.IsMatch(_input);
            _context.AllCorrect = illegal ? !isMatch : isMatch;
            if(_context.AllCorrect)
                return this;

            _context.Errors.Add(Error.Validation(errorCode, errorMessages));
            return this;
        }
    }
}