## Welcome to the Fallen-8 intro
Fallen-8 is an in-memory [graph database](http://en.wikipedia.org/wiki/Graph_database) implemented in C#. Its focus is to provide raw speed for heavy graph algorithms.

## HowTo Clone
Fallen-8-Intro makes use of git submodules, that's why they have to be cloned too.

```
$ git clone --recursive git@github.com:cosh/Fallen-8-Intro.git
```

## HowTo Use

 * compile and start the Fallen-8 Intro project.
 * open http://127.0.0.1:2323/Intro/REST/EquallyDistributed/CreateGraph?nodes=100000&edgesPerNode=20
   * this will create ane equally distributed graph with 100.000 vertices and 2.000.000 edges
 * open http://127.0.0.1:2323/Intro/REST/EquallyDistributed/TPS?iterations=100
   * this will calculate the maximum number of "traversals per second" in 100 iterations
   * during this calculation numberOfIterations * numberOfVertices * numberOfEdgesPerVertex (i.e. 10 * 100000 * 20 = 20.000.000) traversals are executed

## MIT-License
Copyright (c) 2012 Henning Rauch

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
