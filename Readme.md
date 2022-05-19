Sample for: https://github.com/Azure/azure-sdk-for-net/issues/25929


There is main project containing Azure function that would try to override prefetch


Tests include how to override prefetch without setter
but still there is a problem to allow override `MessagingProvider` with my custom version


## To run 
If you want to run this project, please configure connection string: `ServiceBusCon`
it's used for `Function1.cs` to allow to see where is the problem

The expected value would returns:
```
I can decorate 'MessagingProvider'!
```
because that would means that I was able to override that provider

or...
At least that I would receive:
```
Has {providersCount} providers,
where last is used for service bus and it's with type: CustomMessagingProvider
```

where `providersCount` can be `1` (then other provider not registered)
or `2` but it must returns `CustomMessagingProvider` because otherwise services would use other registration

