# SimpleParallelGCD
Code Challenge Taken to Excess

[CodeProject Challenge](https://www.codeproject.com/Answers/1160342/Code-challenge-greatest-common-denominator) to calculate the GCD of a list of integers n where 0 < n <= 10000 in the form {n0, n1, n2,...nx} where x < 500.  Not only did I solve this for 0 <= n <= 2,147,481,557 and x < theMemoryOfYourComputer but I used parallel processing to great effect.  Average running time with 1,000,000 numbers is:

Sequential GCD: ~550-600 ms
Parallel GCD: ~140-160 ms
