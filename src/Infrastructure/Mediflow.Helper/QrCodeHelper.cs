using QRCoder;

namespace Mediflow.Helper;

public static class QrCodeHelper
{
    /// <summary>
    /// Generates a Base64 PNG QR Code for the Parameter Content.
    /// </summary>
    public static string GenerateBase64Png(this string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new Exception("QR content cannot be null or empty.");

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(20);

        return Convert.ToBase64String(qrBytes);
    }
}