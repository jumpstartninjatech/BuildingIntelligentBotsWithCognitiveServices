var request = require('request');
var fs=require('fs');
require('dotenv-extended').load();
var data =fs.readFileSync('./images/satya.jpg');

var faceid;
var personId;
var confidence;


detect();


function detect()
{

  request({
  
  
  headers:{
    'Ocp-Apim-Subscription-Key':process.env.subcriptionkey,
    'Content-Type':'application/octet-stream'
  },
  
  uri:'https://'+process.env.location+'.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=false',
 
  body:data,
  method: 'POST'
}, function (err, res, body) {
  if (err) {
    console.log(err);
  }
  else if(res.body){
  	var temp = JSON.parse(res.body)
     faceid=temp[0].faceId;
    detection();
  }
});

}

function detection()
{

  var temp={
    "personGroupId":process.env.personGroupId,
    "faceIds":[faceid],
    "maxNumOfCandidatesReturned": 10,
    "confidenceThreshold": 0.5
}

var data=JSON.stringify(temp);


   request({
  
  
  headers:{
    'Ocp-Apim-Subscription-Key':process.env.subcriptionkey,
    'Content-Type':'application/json'
  },
  
  uri:'https://'+process.env.location+'.api.cognitive.microsoft.com/face/v1.0/identify',
 
  body:data,
  method: 'POST'
}, function (err, res, body) {
  if (err) {
    console.log(err);
  }
  else if(res.body){
    var temp = JSON.parse(res.body)
    if(temp[0].candidates.length==0)
    {
      console.log("Unknown Person")
    }else{
        personId=temp[0].candidates[0].personId;
        confidence=temp[0].candidates[0].confidence;
        nameextract();
    

    }

  }
});

}

function nameextract()
{


   request({
  
  headers:{
    'Ocp-Apim-Subscription-Key':process.env.subcriptionkey,
  },
  
  uri:'https://'+process.env.location+'.api.cognitive.microsoft.com/face/v1.0/persongroups/'+process.env.personGroupId+'/persons/'+personId,
  method: 'GET'
}, function (err, res, body) {
  if (err) {
    console.log(err);
  }
  else if(res.body){
    var temp=JSON.parse(res.body);
    console.log("-----------Results-------\nName :"+temp.name+"\nPerson ID :"+personId+"\nConfidence :"+confidence);

    
  }
});


}

