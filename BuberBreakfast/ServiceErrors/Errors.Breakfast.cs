using ErrorOr;

namespace BuberBreakfast.ServiceErrors;

public static class Errors
{
    public static class Breakfast
    {
        public static Error InvalidName => Error.Validation(
            code:"Breakfast.InvalidName",
            description:$"Breakfast name length must be {Models.Breakfast.MinNameLength}< <{Models.Breakfast.MaxNameLength}."
        );

        public static Error InvalidDescription => Error.Validation(
            code:"Breakfast.InvalidDescription",
            description:$"Breakfast description length must be {Models.Breakfast.MinDescriptionLength}< <{Models.Breakfast.MaxDescriptionLength}."
        );

        public static Error NotFound => Error.NotFound(
            code:"Breakfast.NotFound",
            description:"Breakfast not found"
        );
    }
}