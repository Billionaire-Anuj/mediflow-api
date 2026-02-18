using System.ComponentModel;

namespace Mediflow.Domain.Common.Enum;

public enum FileType
{
    [Description(".api")] None,
    [Description(".jpg,.png,.jpeg,.gif,.svg")] Image,
    [Description(".mp4")] Video,
    [Description(".mp3")] Audio,
    [Description(".pdf,.xlsx,.doc")] Documents,
    [Description(".com,.net,.org")] Link,
    [Description(".com,.net,.org")] Post,
}