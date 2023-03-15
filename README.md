# BirthdayWisher

[![Hack Together: Microsoft Graph and .NET](https://img.shields.io/badge/Microsoft%20-Hack--Together-orange?style=for-the-badge&logo=microsoft)](https://github.com/microsoft/hack-together)

.net console App which uses microsoft Graph API to Wish them with A birthday Email

HOW TO INITIALIZE 

Step 1: please create ur Azure app see link for more (https://learn.microsoft.com/en-us/graph/tutorials/dotnet-app-only?tabs=aad&tutorial-step=1)

Note:App should have below configuration.
![image](https://user-images.githubusercontent.com/37006391/225288081-2603f45e-90e1-4873-b64a-f9d817992619.png)

Step 2: Add SenderID,client,tenet (senderID is usualy the id of user whose is used to send Emails.

Step 3: Add Secrets in secret store of ur application (use below command)

    dotnet user-secrets init
    
    dotnet user-secrets set settings:clientSecret <client-secret>
    
    
