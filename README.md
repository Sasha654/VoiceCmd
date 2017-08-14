# VoiceCmd
Voice commands for PC control (.NET, C#)

Initially the program has several built-in actions (lock the screen, end the user's session, restart and shut down the PC) and allows you to specify for each such action the text of the voice command for recognition.
The software uses the Russian Microsoft Speech Platform. To use a different language, you must install the appropriate package and specify the appropriate instance of the System.Globalization.CultureInfo class in the code.

![](https://github.com/Sasha654/VoiceCmd/blob/master/Image/01_CmdVoiceFinish.png)     ![](https://github.com/Sasha654/VoiceCmd/blob/master/Image/02_CmdVoiceRun.png)

To run the program, you need to:

1. Microphone 
2. .NET Framework 
2. https://www.microsoft.com/en-us/download/details.aspx?id=27225 - Microsoft Speech Platform - Runtime
   https://www.microsoft.com/en-us/download/details.aspx?id=27226 - Microsoft Speech Platform - Software Development Kit   
   https://www.microsoft.com/en-us/download/details.aspx?id=21924 - Microsoft Speech Platform - Server Runtime Languages (MSSpeech_SR_ru-RU_TELE.msi)

   Performance checked on the Dell Latitude E7470.
