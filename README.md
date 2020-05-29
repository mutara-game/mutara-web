# mutara-web

### Web Application (ASP.NET MVC) for Project Mutara

TODO: write a cool document here.


### Cognito Auto-Confirmation

Amazon Cognito has a thing about users having to go through a full
create account / validate email process that might be OK for a 
website but isn't good for a game with a zero-friction login path.
This might mean that Cognito isn't going to work for this project,
but for now workarounds are still being hacked...

To disable requiring Confirmation, we need to create a Lambda that
Auto-Confirms new users for us. See Docs/Cognito for the Lambda
configuration. Then assign this function to the user pool's
Pre sign-up trigger.
