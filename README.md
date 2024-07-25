![image](https://github.com/user-attachments/assets/f34867d5-7ca4-41b1-981d-cbb41c4ea37a)

This demo program can record a 16 khz WAV file and send it to whisperx for non-realtime (usually higher quality) transcriptions.  It can record multiple WAV files sequentially, and transcribe them asynchronously while you record additional files.  This is a fairly slow way to transcribe audio, as you have to wait for the entire wav file's worth of words to be processed before you can read any of it.

For the demo to work, you will need to have whisperx installed in a (mini)conda environment per instructions on their site:

https://github.com/m-bain/whisperX

Alternatively, this program can be a C# front end for the whisper_streaming server, available here:

https://github.com/ufal/whisper_streaming

WhisperX should already be installed for the streaming server to work.  You will also need to replace the whisper_online_server.py file with the copy from my repo, as it has been slightly modified.

based on OpenAI original Whisper project:

https://github.com/openai/whisper

