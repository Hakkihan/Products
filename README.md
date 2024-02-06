Small Authentication app (.NET 6) using JWT and appropriate unit tests. Estimated time of 3-4 hours. 

Notes:
1) the appsettings.json contains the validAudience, validIssuer and the secret. Ports localhost:7148 and localhost:4200 are used for the audience and issuers. Note that the secret should not be configured in this way (it is vital to encrypt the secret appropriately
   + safe storage eg. AWS secrets manager or Github secrets, etc) 
2) Given more time I would have made a more realistic table of products. For the focus of the task and for the demonstrating of a secured products api endpoint, I simply made a List of readonly
   products within the controller to retrieve.
3) EntityFramework used for user roles, identities, etc.
4) api/authenticate/register-admin should be used to register an admin user to then access the api/productColours endpoint.
   
