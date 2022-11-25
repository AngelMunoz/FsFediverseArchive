# MkSaveNote

This function gets triggered when the topic tied to this function gets fired by any other function (or even in the gcp dashboard)

### The Good

Setting the topic is fairly simple, you need to include the `--trigger-topic <topic-name>` flag when you deploy, you could also create the topic within the function's code but I feel that's really out of scope here

### The Bad

It took me a while to understand that I didn't need to filter out messages by the subscription because the function is really only called when the topic gets published, you also have to ensure to `return! Task.CompletedTask` because any other values could confuse GCP in a way it never finished the execution and hangs up, also be sure to log errors, debug, and warnings properly otherwise you'll find yourself staring at the gcp dashboard trying to figure out why pub/sub events are published but your function doesn't seem to process them

### The Ugly

Firestore is nuts...
Firestore is not designed by any means to work with immutable data, that means you have to fall back to standard classes which aren't so bad in F# but it simply might not be your cup of tea, the good thing here is that no-one is forcing you to use this database! you can use whatever you feel is best for your usecase, in my experiment I just wanted to keep using google's tech to get a hang of it, for a C# project Firestore may be cool, but for F# is not so interesting, the mongodb driver is even better at this than this client library (sadly)
