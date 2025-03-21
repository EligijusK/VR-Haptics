namespace WeArt.Bluetooth
{
    /// <summary>
    /// The container of commands that could be sent to Weart Device.
    /// </summary>
    public static class DeviceCommands
    {
        public static byte[] CONNECT_CONFIRM = new byte[] { 0xAA, 0xE0, 0x00, 0x00, 0xB5 };
        public static byte[] GET_DEVICE_STATUS = new byte[] { 0xAA, 0xF0, 0x00, 0x00, 0xA5 };
        public static byte[] GET_SERIAL_NUMBER = new byte[] { 0xAA, 0xE2, 0x01, 0x00, 0x06, 0xB0 };
        public static byte[] ENABLING_RUNNING_ON = new byte[] { 0xAA, 0xEE, 0x02, 0x00, 0x01, 0x01, 0xB9 };
        public static byte[] ENABLING_RUNNING_OFF = new byte[] { 0xAA, 0xEE, 0x02, 0x00, 0x00, 0x01, 0xB8 };
        public static byte[] ACTUATION_AND_TRACKING = new byte[] { 
            0xAA, 0xEF, 0x20, 0x00,
            0x7E, 0xFF, 0x64, 0x7D, 0x00, 0x7D, 0xFF, 0xFF,
            0x7E, 0xFF, 0x64, 0x7D, 0x00, 0x7D, 0xFF, 0xFF,
            0x7E, 0xFF, 0x64, 0x7D, 0x00, 0x7D, 0xFF, 0xFF,
            0x7E, 0xFF, 0x64, 0x7D, 0x00, 0x7D, 0xFF, 0xFF,
            0x9A };
    }
}
