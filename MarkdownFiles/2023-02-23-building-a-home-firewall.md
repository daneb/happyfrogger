---
title: "Building a Home Firewall"
date: 2023-03-23 15:10:00 +0200
category: tech
---

## The Problem 
For about 3 years I've been running a D-Link DIR-825, a fibre and wireless router provided by my ISP. And here to begins my woe. 
I've been struggling with it's maintenance and software patching, it's long aged and become a dinosaur with no more firmware upgrades.
With the increasing attacks worldwide on networks, I wanted deeper insight to the hardware and software used to defend and connect my home network.

So I decided to poke around my router, and discovered that publicly visible to the world, and not by my configuration, was an SSH service.
It was running with admin privilege and with the default password set by D-Link themselves (which I guessed by the way).

Correlation is not necessarily causation of course, but the abstraction has paid a price. 

![Router](/images/DIR-825.png)

### But first, interesting hack
If you want your router to shutdown daily at a set time. Instead of using cron, you can use the scheduler built into the shutdown command.
```bash
# This will shutdown daily at 22:00
echo "shutdown -h 22:00" >> /etc/sysconfig/rc.local
```
The beauty of this is that you can interrupt the shutdown with:
```bash
shutdown -c
```
And to confirm if the command has fired on startup
```
ps -ef | grep shutdown
```

## Back to the Home Firewall setup
My first step to correcting the problem was to try identify a physical firewall that I could purchase. 
I live in South Africa so choice is limited, but I wanted something entirely cost effective, ability to deep dive into network traffic and
something not made in China. That was the D-Link and it's support was horrible. Sadly, the closest thing was R5000 (Ubiquity) and it didn't seem practical
with the current economic climate.

That sparked an old idea, why not try a Raspberry PI 4. I attempted this before using Ubuntu server but ran into challenges with the wifi speed when it was used a Wireless hotspot.
But I was on holiday now and well, why not. I had a incremental 2 phase plan.

## What where some alternatives to IpFire?
1. I could have stuck with the DLink and overwritten it with [OpenWrt](https://openwrt.org). But sadly it wasn't supported.
2. As mentioned the [Unify](https://scoop.co.za/products/wireless-networking/ubiquiti-unifi/) was R4800.
3. Just use vanilla Linux (Ubuntu etc). This is covered below in the Q&A.

## Why this may be valuable to do?
1. Fine grained control of your network
2. More transparency around what is coming in and out
3. Increase learning in network design and security
4. You don't trust your existing device (it's out of firmware upgrades or really old)

### Phase 1 - Proof of Concept
- Retain the D-Link for speedy wifi access, but sufficiently limit it's capability.
- Use the Pi4 as the gateway and firewall with Raspbian 64
- Get basic masquerading working for everything on the D-Link - basically all wifi connected devices routing through the firewall to the internet.

### Phase 2 - Implement with IpFire
- Once that proved itself, I would replace Raspbian with something more enterprise - IpFire.
- Ensure the networking between the two devices and my home computers internet access functions correctly
- Try get rid of the D-Link and use the Pi4 as a Wireless Hotspot as well and gauge its performance

### My Current Configuration
1. Fibre CPE that provides access through Ethernet cable
2. Fibre Service Provider: Coolideas
3. D-Link DIR 825 with the WAN port connected to the CPE and Wifi Hotspot
4. Raspberry Pi4 
  - 8GB Internal memory
  - 32 GB SSD 
  - USB Broadcom NIC
  - Internal WLAN NIC 
  - Internal Ethernet NIC

![Pi4](/images/Pi4.jpg)

### My Target Configuration
1. No more D-Link router (unless providing access to guests)
2. Raspberry Pi4 running [IpFire](http://ipfire.org)
3. Fibre CPE direct into Pi4 for Internet Access (<span style="color: red">RED</span>)
4. Second USB NIC connected to D-Link router for guest access (<span style="color: green">GREEN</span>)
5. Pi4 as a wireless hotspot (<span style="color: blue">BLUE</span>)

## The Result
Well success of course. But perhaps let me talk through some of the challenges.

## Challenges in Phase 1

#### Should I use the WAN port to connect the Pi4 and D-Link?
- This is firstly not supported as the expectation that both the WAN IP is not the same as the WLAN.
- I used a standard port.

#### Do I have to do any port forwarding or masquerading on the D-Link and should the two devices share a subnet?
- No, there was no configuration needed on the D-Link.
- Yes, I used the USB NIC of the Pi4 (GREEN Zone) I used to link up to the D-Link

#### How should I configure the Pi4 Raspbian OS?
- I plugged the Cat 6 from the Fibre CPE into it the internal Pi4 Nic allowing DHCP to configure the internet IP address through the provider.

#### Did you masquerade?
- Yes, I added a simple rule to masquerade the GREEN zone (link between two devices)
```
# sudo iptables -t nat -A POSTROUTING -o eth1 -j MASQUERADE
```

#### Why did I not stop here?
- It was insecure. I would need to spend a significant amount of time setting the firewall rules and debugging.
- Not to mention, installation a ton of software to calibrate network statistics.

## Challenges in Phase 2
Now that we have a proof of concept, let's move onto IpFire. 

#### Any tips for initial setup of Image onto SD-Card?
- In writing the IpFire image to your SD-card ensure to disable the serial cable requirement so you can set it up easily with HDMI and USB keyboard - you need to edit the uENV.txt file
on the flashed SD Card:
```
# From
SERIAL-CONSOLE=ON 
# To
SERIAL-CONSOLE=OFF
```

#### How did you define the zones?
- I used <span style="color: red">RED</span> for the Internet access (internal nic of Pi4)
- <span style="color: green">GREEN</span> for all traffic from the D-Link router (USB nic to D-Link)
- <span style="color: blue">BLUE</span> for wireless traffic using the Pi4 itself
- Be advised for access from GREEN to RED I used the transparent proxy included with IpFire

### It works

![IpFire](/images/ipfire.png)

It's been an amazing experience thus far, by no means an enterprise grade user interface or corporate backing for sexy bells and whistles.
But it's a Toyota Corolla that does the job and well.

#### Supported features I am getting a ton of value around is:
1. Hardware graphing performance (memory, cpu, load)
2. Firewall logs by country, port and IP
3. Tcpdump on console for troubleshooting and viewing low-level packets
4. Built-in proxy
5. Easy support for multiple networks
6. Wireless Hotspot easy configuration
7. Proxy reports and logs
8. Intrusion detection (multi-provider support)
9. IP Address black lists
10. Pakfire enables safe package installation and in-built plugin support
11. DNS resolution and configuration


## Tip
It may be worthwhile when you are done to review this helpful guide around best practices from [IpFire Blog](https://blog.ipfire.org/post/firewall-configuration-recommendations-for-ipfire-users)
A helpful tutorial that may [guide](https://www.hendgrow.com/ugs/HowTo-install-IPFire-on-a-Raspberry-Pi.pdf) your journey.
