//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

// <toplevel>
using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using Newtonsoft.Json;
using RestSharp;
using samples;
// </toplevel>

namespace MicrosoftSpeechSDKSamples
{
    public class IntentRecognitionSamples
    {
        // Intent recognition using microphone.
        public static async Task RecognitionWithMicrophoneAsync()
        {

            //var model = LanguageUnderstandingModel.FromSubscription( "dd6cc6dd04d24d37a5cd7126012df502", "a5b385ea-5588-4704-872c-6a8e86fd2c5d", "westus");            // <intentRecognitionWithMicrophone>
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription("c3d3c37bfafb4a4ea815a2471c717835", "westus");
            
           // SpeechConfig.FromAuthorizationToken

            // Creates an intent recognizer using microphone as audio input.
            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Language Understanding model using the app id, and adds specific intents from your model
               var model = LanguageUnderstandingModel.FromAppId("a5b385ea-5588-4704-872c-6a8e86fd2c5d");
              recognizer.AddIntent(model, "GetStatus", "id1");
              recognizer.AddIntent(model, "ReportStatus", "id2");
              recognizer.AddIntent(model, "HandleGoodGestures", "id3");
              recognizer.AddIntent(model, "HandleBadGestures", "id4");
              recognizer.AddIntent(model, "TakeAPause", "id5");


                // Starts recognizing.
                Console.WriteLine("Speak out the utterance...");


                // Performs recognition. RecognizeOnceAsync() returns when the first utterance has been recognized,

                // so it is suitable only for single shot recognition like command or query. For long-running
                // recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(true);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    //Console.WriteLine($"    Intent Id: {result.IntentId}.");
                    //Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    var luisresponse = JsonConvert.SerializeObject(result);
                    var luisaccessobject = JsonConvert.DeserializeObject<APIClass>(luisresponse);
                    var luislen = luisaccessobject.entities.Length;
                    var luisentobj = luisaccessobject.entities;

                    if (result.IntentId == "id1")
                    {
                        Console.Write(" Call Get Status Method");

                        var Manager = " ";
                        var Member = " ";

                        for(int j=0; j<luislen; j++)
                        {
                            if(luisentobj[j].entity == "suneetha")
                            {
                                Manager = luisentobj[j].entity;
                            }
                            else
                            {
                                Member = luisentobj[j].entity;
                            }
                        }

                        // Get Status API call
                        var client = new RestClient("http://139.59.105.129:8057/getStatus");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("postman-token", "d553a98d-261f-96cb-fcc6-5ca737cac51c");
                        request.AddHeader("cache-control", "no-cache");
                        request.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request.AddHeader("apikey", "L42345I-N8946G-E55dS-H321-Ka36f33rt");
                        request.AddParameter("application/x-www-form-urlencoded", "membername="+Manager, ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);

                        Console.WriteLine(response.Content);

                        var accessobject = JsonConvert.DeserializeObject<GetStatusClass>(response.Content);

                        if (accessobject.ResponseCode == "100")
                        {
                            var t = accessobject.data.Length;
                            var dataObj = accessobject.data;
                            var MemberId = "0";


                            for (int i = 0; i < t; i++)
                            {
                                if (dataObj[i].MemberName == Member)
                                {
                                    MemberId = dataObj[i].MemberID;
                                }
                            }

                            //ViewMember Status API Call
                            var client1 = new RestClient("http://139.59.105.129:8057/viewMemberStatus");
                            var request1 = new RestRequest(Method.POST);
                            request1.AddHeader("postman-token", "b476aa83-9661-845d-a4eb-9cb58e6c17b7");
                            request1.AddHeader("cache-control", "no-cache");
                            request1.AddHeader("content-type", "application/x-www-form-urlencoded");
                            request1.AddHeader("apikey", "L42345I-N8946G-E55dS-H321-Ka36f33rt");
                            request1.AddParameter("application/x-www-form-urlencoded", "memberid=" + MemberId, ParameterType.RequestBody);
                            IRestResponse response1 = client1.Execute(request1);

                            Console.WriteLine(response1.Content);

                        }

                        Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    }

                    if (result.IntentId == "id2")
                    {
                        Console.Write(" Call Report Status Method");

                        var MemberName = " ";
                        var ProjectName = " ";
                        var Status = " ";

                        for (int k = 0; k < luislen; k++)
                        {
                            if (luisentobj[k].type == "Member")
                            {
                                MemberName = luisentobj[k].entity;
                            }
                            else if(luisentobj[k].type == "Projects")
                            {
                                ProjectName = luisentobj[k].entity;
                            }
                            else if(luisentobj[k].type == "Status Report")
                            {
                                Status = luisentobj[k].entity;
                            }
                        }


                        // Report Status API call
                        var client2 = new RestClient("http://139.59.105.129:8057/reportStatus");
                        var request2 = new RestRequest(Method.POST);
                        request2.AddHeader("postman-token", "92ea4dfb-3002-5891-a4a6-9a02098c6626");
                        request2.AddHeader("cache-control", "no-cache");
                        request2.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request2.AddHeader("apikey", "L42345I-N8946G-E55dS-H321-Ka36f33rt");
                        request2.AddParameter("application/x-www-form-urlencoded", "membername="+MemberName+"&projectname="+ProjectName+"&status="+Status, ParameterType.RequestBody);
                        IRestResponse response2 = client2.Execute(request2);

                        Console.WriteLine(response2.Content);

                        Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    }

                    if (result.IntentId == "id3")
                    {
                        Console.Write("Call Handle Good Gestures Method");
                        Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    }

                    if (result.IntentId == "id4")
                    {
                        Console.Write(" Call Handle Bad Gestures Method");
                        Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    }

                    if (result.IntentId == "id5")
                    {
                        Console.Write(" Call Take A Pause Method");
                        Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    }

                    if (result.IntentId == "Call None Method")
                    {
                        Console.Write(" Call Get Status Method");
                        Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");

                    }

                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }

                }
            }
            // </intentRecognitionWithMicrophone>
        }

        // Continuous intent recognition using file input.
        public static async Task ContinuousRecognitionWithFileAsync()
        {
            // <intentContinuousRecognitionWithFile>
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("YourLanguageUnderstandingSubscriptionKey", "YourLanguageUnderstandingServiceRegion");

            // Creates an intent recognizer using file as audio input.
            // Replace with your own audio file name.
            using (var audioInput = AudioConfig.FromWavFileInput("whatstheweatherlike.wav"))
            {
                using (var recognizer = new IntentRecognizer(config, audioInput))
                {
                    // The TaskCompletionSource to stop recognition.
                    var stopRecognition = new TaskCompletionSource<int>();

                    // Creates a Language Understanding model using the app id, and adds specific intents from your model
                    var model = LanguageUnderstandingModel.FromAppId("YourLanguageUnderstandingAppId");
                    recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName1", "id1");
                    recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName2", "id2");
                    recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName3", "any-IntentId-here");

                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) => {
                        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    };

                    recognizer.Recognized += (s, e) => {
                        if (e.Result.Reason == ResultReason.RecognizedIntent)
                        {
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                            Console.WriteLine($"    Intent Id: {e.Result.IntentId}.");
                            Console.WriteLine($"    Language Understanding JSON: {e.Result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                        }
                        else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                            Console.WriteLine($"    Intent not recognized.");
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    recognizer.Canceled += (s, e) => {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }

                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStarted += (s, e) => {
                        Console.WriteLine("\n    Session started event.");
                    };

                    recognizer.SessionStopped += (s, e) => {
                        Console.WriteLine("\n    Session stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };


                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    Console.WriteLine("Say something...");
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
            // </intentContinuousRecognitionWithFile>
        }

        // Intent recognition in the specified language, using microphone.
        public static async Task RecognitionWithMicrophoneUsingLanguageAsync()
        {
            // <intentRecognitionWithLanguage>
            // Creates an instance of a speech config with specified subscription key
            // and service region. Note that in contrast to other services supported by
            // the Cognitive Services Speech SDK, the Language Understanding service
            // requires a specific subscription key from https://www.luis.ai/.
            // The Language Understanding service calls the required key 'endpoint key'.
            // Once you've obtained it, replace with below with your own Language Understanding subscription key
            // and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("dd6cc6dd04d24d37a5cd7126012df502", "westus");
            var language = "en-us";
            config.SpeechRecognitionLanguage = language;

            // Creates an intent recognizer in the specified language using microphone as audio input.
            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Language Understanding model using the app id, and adds specific intents from your model
                var model = LanguageUnderstandingModel.FromAppId("YourLanguageUnderstandingAppId");
               // recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName1", "id1");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName2", "id2");
                recognizer.AddIntent(model, "YourLanguageUnderstandingIntentName3", "any-IntentId-here");

                // Starts recognizing.
                Console.WriteLine("Say something in " + language + "...");

                // Performs recognition. RecognizeOnceAsync() returns when the first utterance has been recognized,
                // so it is suitable only for single shot recognition like command or query. For long-running
                // recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent Id: {result.IntentId}.");
                    Console.WriteLine($"    Language Understanding JSON: {result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
            // </intentRecognitionWithLanguage>
        }
    }
}
