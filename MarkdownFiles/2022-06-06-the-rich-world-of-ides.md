---
title: "The Rich World Of IDEs"
date: 2022-06-06 20:55:01 +2:00
category: blog 
---

Welcome to the Rich World of IDE's. But before we continue our journey we need to revisit the past, for only when we see the past can we truly understand our present and the beauty of tomorrow.

**As we commence our journey,** we will look at the path that led to their creation,  where we are today, what they will look like tomorrow and how you may be using them today without even recognizing it. BUT ALSO, amidst the giants of IDE's known today, I will also cover some weird, wonderful, hidden gems even some crazy and some life-draining. 

### What is an IDE
An IDE is not a text editor. For example, NotePad++  or Sublime Text do not qualify. Apologies if that's news, but the fundamental mistake is to confuse a Text Editor as an IDE. An IDE consists of three main parts,

A **source code editor**  - a text editor that can assist in writing code with features such as syntax highlighting, auto-completion, checking for bugs as code is being written.

The second component is **local build automation** - utilities that automate simple, repeatable tasks as part of creating a local build or artifact of the software for use by the developer. Examples are, compiling or interpreting source, packaging binary code and running automated tests.

The third component is that of a **debugger**. The ability for graphically moving through the original code while it is running or as it runs.


## The Evolution that spurned the IDE (milestones)
#### 1. The Punch card (manual)
And so we wind back the hand of time to a reasonable start, the punch-card (the evolution of Joseph Marie Jacquards automated loom). The punch card or punched cards, was also known as Hollerith cards where holes where punched to represent computer data and instructions.

**The cards were fed into a card reader connected to a computer, which converted the sequence of holes to digital information.** 

Each card would be punched according to columns, going from 0 on the top left corner down to the bottom left in a near excel manner. Each card could hold only so much data, so multiple cards were input into the card reader in exact order, loaded into memory and thereafter the code executed or submitted to the compiler.

![Punch](/images/punch_card.jpg)

The largest punch card program was from the 1950's SAGE air defense system, which used 62500 punched cards (around 5 MB  of data). As you can imagine, the fear of dropping or getting the cards out of order was REAL. In some cases, it wouldn't be possible to put the program back into order.

Interesting fact - that as of 2012, while punched cards were still obsolete some voting machines still used punched cards to record votes.

#### Dartmouth

![Darth](/images/800px-LGP-30.jc1.jpg)

The next piece in the puzzle leading up to the IDE was the Dartmouth Basic Language.  **Professors John Kemeny and Thomas Kurtz** at Dartmouth College purchased a Royal McBee LGP-30 around 1959 and programmed an implementation of the then popular ALGOL 58 programming language. 

But there was a problem at the time, that only one student could access a computer at a time, and they were expensive. So Kurtz in 1961/62 proposed the following: that all Dartmouth student should have access to computing, it should be free and open-access. Based of that principal they forged forward along with their students and developed the first critical piece, DTSS - the Dartmouth time-sharing operating system. And so began in Autumn of 1964, hundreds of university students began to use the system via 20 teletypes. 

![](/images/600px-ASR-33_Teletype_terminal_IMG_1658.jpg)
DTSS consisted of two machines, the Datanet-30 which provided the user-interface and scheduler, while user programs ran in the GE-225. The GE-225 was later replaced by the faster GE-235 allowing for the support of forty simultaneous users.  Kemeny and Kurtz observed, "any response time which averages more than 10 seconds destroys the illusion of having one's own computer" so the DTSS design emphasized immediate feedback. 

![](/images/20220519061217.png)  

The second crucial piece they contributed at the same time was the BASIC compiler (Beginners All purpose symbolic instruction code). The aim of BASIC was that they wanted to enable students in non-scientific fields to use computers. At the time, nearly all computers required writing custom software, which only scientists and mathematicians tended to learn. Along with DTSS, this proliferated it's usage and adoption.

![](/images/H-20734-2.jpg)

**But why is this important?** Because of the push for ease of use and open-access, they then implemented the world's first hardware IDE. Any line typed in by the user, and beginning with a specific line number; was added to the program replacing any previously stored line with the same number. Anything else was taken and immediately executed. This method of editing provided a simple and easy to use service that allowed large numbers to input at their terminal units for the DTTS.

#### 3.  Maestro I First IDE (software)
We have looked at the necessary advancements that were manual in the punch cards, and the hardware based advancements through DTSS, BASIC and our first IDE. Our next and final stop on this historic journey is that of the Maestro I. 

![](/images/1024px-Maestro-I-Keyboard.jpg)

This was an early software IDE developed by Softlab Munich in the 1970's and 80s. The system was originally called "program development terminal system" abbreviated as PET. 

The advancement here was that unlike the DTSS system we just looked at where instructions were accumulated through time-sharing and edited via IDE commands, the Maestro solved this by feeding each key-stroke directly to the CPU. 

#### 4.  The Pattern of Evolution

![](/images/20220519093534.png)

Just to close of before we move onto the IDE's today, there is quite a debate around the first GUI IDE and not much by history. There seems to be two competing positions, one says SmallTalk and the other Visual Basic. You pick your poison ;)

## Where we are today 

#### 1.  Most prominent IDE's 

![](/images/20220519100803.png)

I'm sure the prominent IDE's of the day need no introduction. Visual Studio and Visual Studio Code holds 43% market share, with Visual Studio having 30%, Eclipse 14% and Visual Studio on 12%. 

PyPL trawls data in Github and community websites, so let's overlay that with direct feedback from over 80 000 developers across the world in 2021. 70% of respondents say they use Visual Studio Code, 33% say they use Visual Studio and 29% Notepad++. 

![](/images/20220519102349.png)

What makes Visual Studio Code so powerful and dominant is firstly it is open-source and cross-platform being largely built in Electron and NodeJS. The second aspect in my opinion, was it's contribution of LSP (language server protocol), providing language intelligence as an open standard.

LSP didn't just unlock doors for Microsoft, Microsoft unlocked doors for other IDE's and editor communities. LSP is basically a JSON-based data exchange protocol for providing **intelligent language services consistently across code editors and IDE's**. 

![](/images/language-server-sequence.png)

LSP enabled the integration of features like auto complete, go to definition, find all references and a host of other intelligence. The best part, is that if the IDE doesn't support a specific language, the community (you and I) now have the ability to implement it and plug it in for whatever weird fetish of a language we may choose. In fact, LSP is not restricted to programming languages alone, it can be used by any kind of text-based language that is custom to you or your business (DSL).

While many slate Microsoft for being many things, one massive contribution is LSP. In fact, the IDE I enjoy most Emacs which was written in the 1970's is now far more accessible because it has integrated LSP allowing it to be open to languages like C#, Go, Java etc and not just LISP.

### 2.  Some crazy ides
I would be re-miss if I didn't draw attention to two very large communities, and that of Vim and Emacs. While some may consider Visual queue IDE's as the only future, there are still two very large communities for Vim and Emacs that is keyboard-driven.

![](/images/initialNvim.png)

In fact, large communities have formed around them and modernized them for every day use. For Vim you have Neovim, which has about 800 contributors in comparison to VSCode which has 1200. What makes vim so attractive is that it's base form is found on almost every installation of Linux. So if you involved in DevOps, Developer or hobbyist using Linux you probably stumbled onto it, either famously failing to quit it or falling in love.

![](/images/R.png)

Then for Emacs there is SpaceMacs, with a very active community 100 short of VsCode with 1100 contributors.  Emacs draw card is that it is highly extensible and touts three features that are specific to it, TRAMP which enables remote editing of files, Org-Mode which enables rapid note taking, authoring documents, computational notebooks, literate programming, maintaining to-do lists etc. The third is Magit, which is an application that allows the perfect balance of UI and Console interaction with Git or git providers. Emacs community is so enthusiastic about their IDE there's even a Window Manager called EXWM that managers all your windows from your browser to anything in your Linux Desktop so much so, that you never leave Emacs.

![](/images/R1.png)

So why use one of these two editors? Well aside from their own individual features, they allow for rapid navigation, an array of keyboard shortcuts and customization. 

### 3.  IDE's breaking barriers
#### Khan Academy

I've been recently looking at introducing my 7 and 9 year old into programming and there are amazing tools. While I've taken the journey of starting them off on a board game that teaches elementary logic needed for programming, I've now found more advanced engagement with Drawing and Animation course @ Khan Academy. It's provides a dumbed down UI that does syntax highlighting, basic debugging, building and rendering. It's amazing. IDE's are now becoming tools for expression of ideas, creativity and even design.

This picture is by my 9 year old, it's actually animated and is called parting clouds. She's a passionate engineer whereas my second is a super creative pixies and fairy land kind of dreamer, and both were amazed by what they were able to accomplish with a few lines of code - magic on screen.

![](/images/20220519160318.png)

#### Scratch 

As I mentioned my 9 year old has been demanding to learn more about engineering and I stumbled onto this little device. It connects to your PC with USB (entirely plug and play) and has a speaker, buttons on the left and right to take clicks as input, has those LED's to displays messages, has a gyro and a mic and event a touch sensor. I was amazed and I wanted one, but what blew me away even further, was that they made this entirely accessible to kids through a Scratch type IDE. In this case, it's MakeCode by Microsoft.

![](/images/led-screen-27qu5u8.png)

![](/images/20220519161433.png)

By dragging simple blocks you are able to interact with the micro-bit, turn on a servo, make a motion detection camera - get your kid to built out their imagination, and the best part, they can code it themselves.

## The advancements of tomorrow

#### 1. AI In the IDE

Some of the advancements we are seeing in IDE's in 2022 is that of the introduction of AI. AI in the IDE is definitely not new, but during it's current boom the technology has definitely come to the forefront, with 5 players in Github CoPilot, Kite, Intellicode, TabNine and OpenAI Codex.

So how do they assist? Largely the guidance and code completion comes from patterns learned from millions of public repositories and projects.  If you are considering using the tooling there are some important questions:

(a) Is the artificial intelligence distributed or local? Is your code going over the wire?  
(b) What is the code quality given it's based on open source projects? How long before it does produce quality?  
(c) Most time with code generation tools, you end up with more code than you intended.  
(d) In terms of business development, is there any breaches in license or legal implications in using code found in the wild.  
(e) According to OpenAI, Codex only gives the correct answer 29% of the time.  

Overall, this can only get better. Consider for a moment, a company onboarding juniors and in their daily jobs, junior and new developers are given automated code-assistance on best practices and suggestions trained by models within the organization and it's senior developers. Suddenly, we scale people, we accelerate growth and we minimize friction!

![](/images/20220519171808.png)

![](/images/20220519171958.png)

#### 2. No Code

There's two aspects here and they often get conflated, there's No-code and Low-code. Low-code is ability for developers to accelerate their development through minimal code.
No-code on the other hand is catered to business people or others in IT that don't know a programming language and able to build systems through drag-and-drop or visual development.

Gartner predicts that by 2023, over 50% of medium to large enterprises will have adopted low-code or no-code as one of their strategic platforms and low-code will be responsible for more than 65% of application development by 2024.

![](/images/No-code-low-code.webp)


# Conclusion

The world of IDE's has indeed changed and continues to change, but if anything there is a **bright future ahead**. But while there is much to look forward by way of advancements in AI and Machine Learning, let us not fall to see the **privilege** and **opportunity** we have today. There is a **very low barrier** to allowing your **creative** juices to flow and find an expression of who you are through code.

IDE's have taken many forms. Scratch and block-code has opened up a whole new world for kids to be able to code and create themselves. Makecode has opened up  the world of engineering, computing, home automation to everyone. AI and Low-code are enabling developers to accelerate in the work-place and whose potential we are unlocking. IDE's themselves have garnished communities and contributors who are able to impact and improve productivity of millions. Opportunity is around. Creativity abounds, and all that is missing from the puzzle is YOU.

![](/images/20220519193200.png)

Go out and create!

### References
1. [Dartmouth BASIC]()
2. [Teletypes](https://webot.org/info/en/?search=Teleprinter)
3. [Maestro I](https://en.wikipedia.org/wiki/Maestro_I#/media/File:Maestro-I-Keyboard.JPG)
4. [PyPL Index](https://pypl.github.io/IDE.html)
5. [2021 Stackoverflow Survey](https://insights.stackoverflow.com/survey/2021)
6. [Intellicode](https://github.com/MicrosoftDocs/intellicode/issues/102)
7. [Fast.ai blessing or curse?](https://www.fast.ai/2021/07/19/copilot/)


