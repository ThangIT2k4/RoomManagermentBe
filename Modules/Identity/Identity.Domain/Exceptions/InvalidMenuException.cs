using Identity.Domain.Common;

namespace Identity.Domain.Exceptions;

public sealed class InvalidMenuException(string errorCode) : DomainException(errorCode, GetMessage(errorCode))
{
    public const string CodeEmpty = "MENU_CODE_EMPTY";
    public const string CodeTooLong = "MENU_CODE_TOO_LONG";
    public const string LabelEmpty = "MENU_LABEL_EMPTY";
    public const string LabelTooLong = "MENU_LABEL_TOO_LONG";
    public const string PathEmpty = "MENU_PATH_EMPTY";
    public const string PathTooLong = "MENU_PATH_TOO_LONG";
    public const string IconEmpty = "MENU_ICON_EMPTY";
    public const string IconTooLong = "MENU_ICON_TOO_LONG";

    private static string GetMessage(string errorCode) => errorCode switch
    {
        CodeEmpty => "Menu code cannot be empty.",
        CodeTooLong => "Menu code exceeds maximum length.",
        LabelEmpty => "Menu label cannot be empty.",
        LabelTooLong => "Menu label exceeds maximum length.",
        PathEmpty => "Menu path cannot be empty.",
        PathTooLong => "Menu path exceeds maximum length.",
        IconEmpty => "Menu icon cannot be empty.",
        IconTooLong => "Menu icon exceeds maximum length.",
        _ => "Menu validation failed."
    };
}
