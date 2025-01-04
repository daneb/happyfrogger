---
title: "What I learned this week"
date: 2023-01-11 07:10:00 +0200
category: creative
subcategory: wiltw
description: Skills development
slug: wiltw-2023-01-11
---

### Quotes:

#### Tiny Thought
> If you wait until you're motivated, you’ve already lost.
> Surgeons don’t always feel like doing surgery. Teachers don’t always feel like teaching. Parents don’t always feel like cooking. Firemen don’t always feel like rushing into a burning building.
> If you let motivation dictate your actions, inertia conspires to keep you in place.

> Action creates progress. Progress creates momentum. Momentum creates motivation.

### CAP Theorem
Your two choices: you have to choose between consistency or availability. You cannot have both.
This assumes 100% availability but you could have degrees of consistency along with availability (or vice versa).

### List.ForEach is not foreach (C#)
List.foreach will create isolated clojures that run Actions independently without side-affects.
foreach is synchronous and mutates.
[Eric Lippert](https://learn.microsoft.com/en-us/archive/blogs/ericlippert/foreach-vs-foreach)

### Redis Use Cases (blog.bytebytego.com)

![Use cases](/images/redis.jpg)

### Playwright
- For Playwright tests in .NET, there is no need to use Playwright directly. But you can use Playwright.NUnit to implement and use Page.

### Adding a User to SQL Server (some crucial steps) to connect from Docker with
1. Ensure you checkout SQL Configuration Manager - Is it running via TCP as a protocol?
1. Don't forget to enable TCP listening on the IP you require
1. User gets added to Sql Server
1. User then get's given permission in database with Securables (schemas)

### Team Lead Tid-bit:
> Never forget to keep your eye on the big picture when leading a team. It's easy when fighting fires and trust is low to get stuck in the trenches.
> In the trenches, strategy is missed and often times that leads to a disconnect in what business needs and what is visible to business.

### Measuring team performance:
* When it comes to measuring a teams performance and reporting back to business, don't build your own metrics.
* Often times the metrics that drives a team internally are not the same metrics that provide business the value they need.
* How to start measuring a team for yourself? Start by figuring out how you would measure yourself.

