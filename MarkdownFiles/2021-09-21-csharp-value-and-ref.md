---
title: "C# Value and Reference Types"
date: 2021-09-21 10:22:22 +0200
category: tech
---

**Value** and **Reference** types are covered frequently all over the internet, in blog posts, technical articles and forums. Why? Because it's easy to develop software for a long time without stopping to 'smell the roses'. Or in this case, understand what they are and how they work. So here's a note to self, or you.

> A **Value Type** is some object that is the information itself.

> A **Reference Type** is the details which points to an object containing the information. The details include things like type of object etc.

## An Analogy:

Let's set the table with an analogy. Kim has just written a book. It's an auto-biography. She would like to get feedback so she prints a copy at home and gives it to her husband Kanya. Later that day she stumbles onto a publicist, who is intrigued and asks to read it. She fires up her Macbook and shares the link to the original via Google Drive.

As time passes the publicist feeds back and is thoroughly impressed. She has highlighted a slew of spelling mistakes and a consideration for re-writing Chapter 4. Kim goes to work immediately and in a few days is done. Updating the original and requesting that the publicist review her changes.

**So what's this got to do with value and reference types?**
Well, when Kim made Kanya a physical copy of the original. She provided him with an independent version of the truth. In fact after all her changes during her engagement with the publicist, Kanya's copy has now become outdated and obsolete. He had a **value type**. An independent copy of the source, that was detached and indifferent to modifications of the source.

The publicist on the other hand, well she had a Google Drive URL, a **reference** to the original. In fact as Kim made changes the publicist could see those changes in real time. The **reference** provided access to the original but was not the original.

## Some Examples

- Array types are reference types, even if the element type is a value type (so int[] is still a reference type, even though **int** is a value type)
- Enumerations are value types.
- Delegates are reference types.
- Interfaces are reference types, but they can be implemented as values.

## Stack vs Heap

> References types live on the Heap.
> Value types lives where their declared.
> Local variables and method parameters live on the Stack.

A **Stack** is a simple FIFO memory structure. When a method is invoked, the CLR bookmarks the top of the stack. The method then pushes data onto the stack as it executes. When the method completes the CLR just resets the stack to it's previous bookmark.

### Stack offers:

- very fast access
- space is managed efficiently by CPU, memory will not be fragmented
- local variables only
- limit on memory size

A **Heap** is a random jumble of objects not managed by automatically for you. It is a free-floating region of memory (and is larger). In multi-threaded applications each thread will have its own stack. But all the different threads will share the heap.

### Heap Offers:

- variables can be accessed globally
- no limit on memory size
- (relatively) slower access

It's critical to note, that a value type lives where it's declared, so if you have a class with an instance variable of float, a value type. It will live wherever the rest of the object is found and that is on the heap.

A last point, is that a string in C# is a reference type. Why? Because strings can be grow quite large in size and thus have to stored on the heap.

## References

1. [Reference Types and Value Types in C - Vegitbit](https://vegibit.com/reference-types-and-value-types-in-c/)
2. [C# In Depth - Jon Skeet](https://csharpindepth.com/)
3. [Values vs Reference Types - Joseph Albahari](http://www.albahari.com/valuevsreftypes.aspx)
4. [Difference between Stack and Heap](https://www.programmerinterview.com/data-structures/difference-between-stack-and-heap/)
