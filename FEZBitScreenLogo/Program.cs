using System;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;
using GHIElectronics.TinyCLR.Pins;
using System.Drawing;
using GHIElectronics.TinyCLR.Drivers.Worldsemi.WS2812;
using System.Threading;



ST7735Controller st7735 = null;
void InitDisplay()
{
    // Display Get Ready ////////////////////////////////////
    var spi = SpiController.FromName(FEZBit.SpiBus.Display);
    var gpio = GpioController.GetDefault();
    st7735 = new ST7735Controller(
    spi.GetDevice(ST7735Controller.GetConnectionSettings
    (SpiChipSelectType.Gpio,
    gpio.OpenPin(FEZBit.GpioPin.DisplayChipselect))), //CS pin.
    gpio.OpenPin(FEZBit.GpioPin.DisplayRs), //RS pin.
    gpio.OpenPin(FEZBit.GpioPin.DisplayReset) //RESET pin.
    );
    var backlight = gpio.OpenPin(FEZBit.GpioPin.Backlight);
    backlight.SetDriveMode(GpioPinDriveMode.Output);
    backlight.Write(GpioPinValue.High);
    st7735.SetDataAccessControl(true, true, false, false); //Rotate the screen.
    st7735.SetDrawWindow(0, 0, 160, 128);
    st7735.Enable();
}

void Graphics_OnFlushEvent(Graphics sender, byte[] data, int x, int y, int width, int
height, int originalWidth)
{
    st7735.DrawBuffer(data);
}

InitDisplay();

const int NUM_LED = 8;
var pin = GpioController.GetDefault().OpenPin(FEZBit.GpioPin.P0);
var leds = new WS2812Controller(pin, NUM_LED, WS2812Controller.DataFormat.rgb888);

Graphics.OnFlushEvent += Graphics_OnFlushEvent;
var screen = Graphics.FromImage(new Bitmap(160, 128));
screen.Clear();
var image = FEZBitScreenLogo.Resources.GetBitmap(FEZBitScreenLogo.Resources.BitmapResources.logo);
screen.DrawImage(image, 0, 0);
screen.Flush();



