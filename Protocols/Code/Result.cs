using EnumExtend;

namespace Protocols.Code
{
    [DescriptiveEnumEnforcement(DescriptiveEnumEnforcementAttribute.EnforcementTypeEnum.DefaultToValue)]
    public enum Result
    {
        [Description("성공")]
        Success,

        [Description("구현되지 않은 기능입니다")]
        NotImplementedYet,

        [Description("유효하지 않은 닉네임입니다.")]
        InvalidNickname,

        [Description("닉네임이 중복되었습니다.")]
        DuplicateNickname,

        [Description("닉네임이 존재하지 않습니다.")]
        NotHaveNickname,

        [Description("서버 내부 예외가 발생했습니다")]
        Exception,

        [Description("알려지지 않은 오류입니다")]
        Unknown,
    }
}