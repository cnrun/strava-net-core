Strava.NET
============

NOTE: Visit the new homepage at www.sascha-simon.com for more information and updates on the Strava Client using the framework.

Current version: 3.3.0 (2015-09-10)

You can now check out the Samples-Project: [Samples-project on GitHub](https://github.com/inexcitus/stravasamples)

[Changelog](http://www.sascha-simon.com/Changelog)

Hopefully this small framework makes using the Strava API a little bit easier!
Please keep in mind that this framework is in a pretty early stage and things are about to change.
Kudos to Strava for letting people use data uploaded to Strava! Strava.NET uses the great Json.NET library from James Newton-King - such a great piece of code.

You can get the framework by using NuGet:

    PM> Install-Package Strava.NET

Feedback
============

If you have any feedback, bug reports or suggestions, feel free to send me an email.
My mail address is strava (at) sascha-simon (dot) com.


Getting an access token from Strava
============

You can get an access token from Strava by using one of the following methods:

- StaticAuthentication
  You can use this method if you already have an access token. You can either use the WebAuthentication class to get a access token from Strava or you can use your token, that you got when you registered your application.  
    
```C#
StaticAuthentication auth = new StaticAuthentication("<insert token here>");
```

- WebAuthentication
  This procedure should be used, if you want to authorize your application for the first time. When an object is created and the *GetTokenAsync()* method is invoked, a browser window will open and you have to authorize the apllication. Once the button is clicked, Strava invokes a callback function. When you create a WebAuthentication object, a web server is started locally on your computer. Once the button is clicked, Strava invokes a callback function which is received by the callback server. You now have a working access token created specifically for your application. You can store this access token in a file on your hard disk, so you don't have to open a browser window every time. You should use some sort of cryptographic algorithm, to obfuscate the access token.

Upon the next start of the program, you can then load the token from your hard disk and use the StaticAuthentication method described above.


Hints
--------------

When using the WebAuthentication method in your application, you have to start the application as an admin (at least when you acquire the access token). Strava sends back the access token via a callback. To receive this callback, a WebServer must be started locally. To start such a server, you need admin rights.

After you have received your token, you can save it to a file so you won't have to get a new token every time.

Using Strava.NET
============

General
--------------

Getting data from Strava is pretty straightforward. All you have to do, is to create a *StravaClient* object and pass a valid IAuthenticator object.
    
```C#
StaticAuthentication auth = new StaticAuthentication("<insert token here>");
StravaClient client = new StravaClient(auth);
```
    
Now you can use the *client* object to make some calls to Strava.

As of now, i only have implemented the async methods, regular methods will follow.
Most of the methods are overwritten. When you don't need to pass a parameter to the method, the data will be of the currently authenticated athlete.

```C#
//Receive the currently authorized athlete
Athlete athlete = await client.GetAthleteAsync();
```
  
When you pass a parameter to the method, you can get data from another athlete.
    
```C#
//Receive an other athlete
Athlete athlete = await client.GetAthleteAsync("1985994");
```

Limits
--------------
The usage of the Strava API is limited. There are two different limits, a short- and a long-term limit.
The long term limit is 30.000 request per day, the short term limit is 600 request per 15 minutes. Whenever a request is made and an response is received, the limits are processed and saved in the static properties of the *Limit* class.

This class provides the following members:

| Property                    | Description                                                |
| :-------------------------- | :--------------------------------------------------------: |
| Usage                       | The Usage data object has two properties, the short- and the long term usage. |

The Limit class also provides events you can subscribe to.


Athletes
============

The *StravaClient* offers the following methods:

| Method                                   | Return type | Description                                                |
| :--------------------------------------- | :---------: | :--------------------------------------------------------: |
| GetAthleteAsync()                        | Athlete     | Gets the currently authenticated athlete.                  |
| GetAthleteAsync(String athleteId)        | Athlete     | Gets the profile from any athlete.                         |
| GetFriendsAsync()                        | List&lt;Athlete&gt; | Gets the friends of the currently authenticated athlete. |
| GetFriendsAsync(String athleteId)        | List&lt;Athlete&gt; | Gets the friends of any athlete.                   |
| GetFollowersAsync()                      | List&lt;Athlete&gt; | Gets the followers from the currently authenticated athlete.         |
| GetFollowersAsync(String athleteId)      | List&lt;Athlete&gt; | Gets the followers of any athlete.                 |
| GetBothFollowingAsync(String atheleteId) | List&lt;Athlete&gt; | Gets the athletes that both you and any athlete are following.       |
| UpdateAthlete(AthleteParameter parameter, String value) | Athlete | Updates a specified parameter of the currently authenticated athlete. Requires **write** permissions. |
| UpdateAthleteSex(Gender gender) | Athlete | Updates the sex of the currently authenticated athlete. Requires **write** permissions. |

Examples
--------------

```C#
StaticAuthentication auth = new StaticAuthentication("<token here>");
StravaClient client = new StravaClient(auth);

//Receive the currently authenticated athlete
Athlete athlete = await client.GetAthleteAsync();
```

```C#
StaticAuthentication auth = new StaticAuthentication("<token here>");
StravaClient client = new StravaClient(auth);

//Receive an other athlete
Athlete athlete = await client.GetAthleteAsync("1985994");
```

```C#
StaticAuthentication auth = new StaticAuthentication("<token here>");
StravaClient client = new StravaClient(auth);

//Get my followers
List<Athlete> athlete = await client.GetFollowersAsync();
```


Activities
============

There are three types of activity objects:

- Activity
- ActivitySummary
- ActivityMeta

The *Activity* object contains all the information about an activity. An *Activity* object is only returned if you are the owner of the activity otherwise an *ActivitySummary* is returned.

An *ActivityMeta* object only contains the activity id and is returned if you load other data like a segment effort. The *SegmentEffort* object only contains an *ActivityMeta* member with the activity id. But you can use this id to get a more detailed version of the activity.

The *AcitivtySummary* and *ActivityMeta* classes were added to avoid some nasty NullPointerExceptions.


| Method                                   | Return type | Description                                                |
| :--------------------------------------- | :---------: | :--------------------------------------------------------: |
| GetActivityAsync(string id) | Activity or ActivitySummary | Gets a detailed version of an activity if you are the owner of the activity. Otherwise an activity summary is returned. |
| GetCommentsAsync(string activityId) | List&lt;Comment&gt; | Gets all the comments of the specified activity. |
| GetKudosAsync(string activityId) | List&lt;AthleteSummary&gt; | Gets a list of athletes that kudoed the specified activity |
| GetActivityZonesAsync(String activityId) | List&lt;ActivityZone&gt; | Gets a list of ActivityZones for the specified activity. (**Premium Feature**)
| GetActivityBeforeAsync(String id, DateTime before) | List&lt;ActivitySummary&gt; | Gets a list of activities that were recorded before the specified date. |
| GetActivityAfterAsync(String id, DateTime after) | List&lt;ActivitySummary&gt; | Gets a list of activities that were recorded after the specified date. |
| DeleteActivity(String activityId) | void | Deletes the specified activity. Requires you to be the *owner* of the activity and **write** permissions. |

Examples
--------------

```C#
StaticAuthentication auth = new StaticAuthentication("<token here>");
StravaClient client = new StravaClient(auth);

//Receive an activity
Activity athlete = await client.GetActivityAsync("102162300");
```

```C#
StaticAuthentication auth = new StaticAuthentication("<token here>");
StravaClient client = new StravaClient(auth);

//Receive all the comments
List<Comment> comments = await client.GetCommentsAsync("102162300");

foreach (Comment comment in comments)
{
    Console.WriteLine(String.Format("{0} {1} says '{2}'", comment.Athlete.FirstName, comment.Athlete.LastName, comment.Text));
}
```

Clubs
============

| Method                                   | Return type | Description                                                |
| :--------------------------------------- | :---------: | :--------------------------------------------------------: |
| GetClubAsync(String clubId) | Club | Gets the club with the specified id. The club must either be public or you must be a member of the club to receive some data. |
| GetClubsAsync() | List&lt;Club&gt; | Gets all the clubs of the currently authenticated athlete |
| GetClubMembersAsync(String clubId) | List&lt;AthleteSummary&gt; | Gets all the members of the specified club. |


Gear
============

Segments
============

| Method                                   | Return type | Description                                                |
| :--------------------------------------- | :---------: | :--------------------------------------------------------: |
| GetStarredSegmentsAsync() | List&lt;SegmentSummary&gt; | Returns a list of segments that the currently authenticated athlete has starred. |


Segment Efforts
============

Leaderboards
============

You can filter the leaderboard by various parameters like gender or weight. Please keep in mind that you need a Strava premium account to use filtering.

| Method                                   | Return type | Description                                                |
| :--------------------------------------- | :---------: | :--------------------------------------------------------: |
| GetFullSegmentLeaderboardAsync(string segmentId) | Leaderboard | Returns the unfiltered leaderboard for the specified segment. |
| GetSegmentLeaderboardAsync(string segmentId, Gender gender) | Leaderboard | Returns the leaderboard for the specified segment, filtered by gender. |
| GetSegmentLeaderboardAsync(string segmentId, Gender gender, AgeGroup age) | Leaderboard | Returns the leaderboard for the specified segment, filtered by both gender and age group. (**Premium Feature**) |
| GetSegmentLeaderboardAsync(string segmentId, Gender gender, WeightClass weight) | Leaderboard | Returns the leaderboard for the specified segment, filtered by both gender and weight class. (**Premium Feature**) |


Uploading an activity
============

You can use the *UploadClient* to upload an activity from a *.fit, *.tcx or a *.gpx file.
I recommend that you create a *StravaClient* and use the predefined subclients.

After you've defined the file you want to upload, you can use the *UploadStatusCheck* class to check the status of your upload. There are events that are raised when a certain status of the upload is reached.

```C#
StaticAuthentication auth = new StaticAuthentication("<your token>");
Client.StravaClient client = new Client.StravaClient(auth);

UploadStatus status = await client.Uploads.UploadActivityAsync(@"F:\2014-03-22-14-43-55.fit", DataFormat.Fit);

UploadStatus s = await client.Uploads.CheckUploadStatusAsync(status.Id.ToString());
Console.WriteLine(s.Status);

UploadStatusCheck check = new UploadStatusCheck(_token, status.Id.ToString());
            
check.UploadChecked += delegate(object o, UploadStatusCheckedEventArgs args)
{
    Console.WriteLine(args.Status);
};

check.Start();
```



