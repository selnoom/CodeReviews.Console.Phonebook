# Phonebook App

Please make sure to confiure Twilio correctly in the appsettings.json folder if you wish to send an SMS. Also, since I am from Brazil, I configured the program to send SMS only from Brazilian numbers (11 digits). Add the code below to the file to make it work:

{

  "Twilio": {
  
    "AccountSid": "YOUR_ACCOUNT_SID_HERE",
    
    "AuthToken": "YOUR_AUTH_TOKEN_HERE",
    
    "FromNumber": "+55XXXXXXXXXX"
    
  }
}
