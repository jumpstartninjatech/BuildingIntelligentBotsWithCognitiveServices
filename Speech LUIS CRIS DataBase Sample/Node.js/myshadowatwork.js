var request = require('request');
var fs=require('fs');

var prompt = require('prompt');
var CustomSpeechText;
var LUISJson;
var TopScoringIntent;
var AuthorityName;
var MemberName;
var MemberID;
var projectName;
var statusreport;


console.log("Please select any option to perform operation.");
console.log("1.Report status");
console.log("2.Get Status")
prompt.get(['Choice'], function (err, result) {

text=result.Text;


if((result.Choice==1)||(result.Choice==2)){
data =fs.readFileSync('./voice_sample/'+result.Choice+'.wav');
customSpeech();
}
    
  });




function customSpeech()
{

request({
  
  headers:{
    'Ocp-Apim-Subscription-Key':"1b08bc65feeb45ee821f6dcb8ac72db7",
     'Content-Type':'application/octet-stream'
  },  
  uri:'https://westus.stt.speech.microsoft.com/speech/recognition/dictation/cognitiveservices/v1?cid=a15561ad-096e-4e95-ba7c-368425d627a7',
  body:data,
  method: 'POST'
}, function (err, res, body) {
  if (err) {
    console.log(err);
  }
  else {
    var temp=JSON.parse(body)
    CustomSpeechText=temp.DisplayText;
    console.log("**custom Speech Service**\nIdentified Utterence:",temp.DisplayText);
    luisapi();
  }
});

}

function luisapi(){


  request({
  
  headers:{
   
  },  
  uri:'https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/a5b385ea-5588-4704-872c-6a8e86fd2c5d?subscription-key=c3d3c37bfafb4a4ea815a2471c717835&timezoneOffset=-360&q='+CustomSpeechText,
  method: 'GET'
}, function (err, res, body) {
  if (err) {
    console.log(err);
  }
  else {
    var temp=JSON.parse(body);
    LUISJson=temp;
    
    if(LUISJson.query==null)
    {
      Console.log("Utterence is not given as input in API");
    }else{

      TopScoringIntent=LUISJson.topScoringIntent.intent;
      console.log("Top Scoring Intent :",TopScoringIntent);
       if(TopScoringIntent=='GetStatus')
       {
                     var Member=false;
                     var Authority=false;


                     if(LUISJson.entities.length>0)
                     {
                      for(var i=0;i<LUISJson.entities.length;i++){
                            if((LUISJson.entities[i].type=='Member')&&(LUISJson.entities[i].entity=='suneetha')){
                                Authority=true;
                                AuthorityName=LUISJson.entities[i].entity;
                            }else if((LUISJson.entities[i].type=='Member')&&(LUISJson.entities[i].entity!='suneetha')){
                              Member=true;
                              MemberName=LUISJson.entities[i].entity;

                            }

                      }
                      
                      GetStatusFromDb();
         }

 
       }else if(TopScoringIntent=='ReportStatus'){
          
            var Member=false;
            var Project=false;
            var status=false;


                 if(LUISJson.entities.length>0)
                 {
                  for(var i=0;i<LUISJson.entities.length;i++){
                        if(LUISJson.entities[i].type=='Member'){
                          Member=true;
                          MemberName=LUISJson.entities[i].entity;
                        }else if(LUISJson.entities[i].type=='Projects'){
                          Project=true;
                          projectName=LUISJson.entities[i].entity;
                        }else if(LUISJson.entities[i].type=='Status Report'){
                          status=true;
                          statusreport=LUISJson.entities[i].entity;        
                          
                        }

                    }    

                  }
                

                  if((Member==true)&&(Project==true)&&(status==true)){
                    savestatusdb();
                  }

       }

    }
  }
});


}



function savestatusdb(){

var options = { method: 'POST',
  url: 'http://139.59.105.129:8057/reportStatus',
  headers: 
   { 'postman-token': '8958ba7c-8953-a06a-3fe8-38f96e126cd4',
     'cache-control': 'no-cache',
     'content-type': 'application/x-www-form-urlencoded',
     apikey: 'L42345I-N8946G-E55dS-H321-Ka36f33rt' },
  form: 
   { membername: MemberName,
     projectname:projectName,
     status: statusreport} };

request(options, function (error, response, body) {
  if (error){
    console.log(error);
  }else{
    
    var temp=JSON.parse(body);
    if(temp.ResponseCode==100){
      console.log("Your Status of project is successfully Submitted.")
    }else{
      console.log("Invalid Response from API.")
    }

  }


});


}












function GetStatusFromDb(){
 

  var options = { method: 'POST',
  url: 'http://139.59.105.129:8057/getStatus',
  headers: 
   { 'postman-token': '79203c08-7b37-e950-40e4-340f0d275ebb',
     'cache-control': 'no-cache',
     'content-type': 'application/x-www-form-urlencoded',
     apikey: 'L42345I-N8946G-E55dS-H321-Ka36f33rt' },
  form: { membername: AuthorityName } };

request(options, function (error, response, body) {
  if (error){
    console.log("Error :",error);
  }else{
    var temp=JSON.parse(body)
    if(temp.ResponseCode==100){
      
      if(temp.data.length>0)
      {
        var m;
        for(var i=0;i<temp.data.length;i++)
        {
          m=temp.data[i].MemberName;
          if(m.toLowerCase()==MemberName){
        
            MemberID=temp.data[i].MemberID;
          }
        }
     
      GetStatusFromDbviaID();
      }else{
        Console.log("Invalid Response from API.")
      }
      
        

    }else{
      console.log("Invalid Response from API.")
    }

  }

  
});


}


function GetStatusFromDbviaID(){

  var options = { method: 'POST',
  url: 'http://139.59.105.129:8057/viewMemberStatus',
  headers: 
   { 'postman-token': 'cb220a51-3bc6-6e88-b323-a5ed5c93a23f',
     'cache-control': 'no-cache',
     'content-type': 'application/x-www-form-urlencoded',
     apikey: 'L42345I-N8946G-E55dS-H321-Ka36f33rt' },
  form: { memberid: MemberID } };

request(options, function (error, response, body) {
  if (error){
  console.log("Error :",error);

  }else{
    var temp=JSON.parse(body)
    
      if(temp.ResponseCode==100){
        console.log("-------Project Status Report--------\nProject Name:"+temp.data.projectName+"\nProject Status:"+temp.data.Status);

      }else{
        console.log("Invalid Response from API.")
      }
  }


});




}




