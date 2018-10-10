var request=require("request");
require('dotenv').config()
var speechService = require('ms-bing-speech-service');

console.log(process.env.Bing_Speech_Api_Key);

    
  var options = {
  language: 'en-US',
  subscriptionKey:process.env.Bing_Speech_Api_Key
};
 
const recognizer = new speechService(options);
 
recognizer
  .start()
  .then(_ => {
    recognizer.on('recognition', (e) => {
      if (e.RecognitionStatus === 'Success') console.log("Text Detected :"+e.DisplayText);
    });
 
    recognizer.sendFile('sample.wav')
      .then(_ => console.log('file sent.'))
      .catch(console.error);
  
}).catch(console.error)


