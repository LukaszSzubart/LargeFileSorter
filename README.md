# LargeFileSorter
## Task
1. Prepare a program which will create text file of a given size. Each line should match format: ```Number. String``` e.g.
```
5324. Shadow 
30432. Something something something 
1. Apple 
32. Cherry is the best 
2. Banana is yellow
```
2. Prepare a program which will sort a file from part 1.
- file should be of size 1GB, 10GB and 100GB
- sorting criteria: String is compared first thern number

## Restrictions and initial design decisions
- Max memory usage: 1GB 
- Encoding: ASCII
- Max line count per chunk: 1024*1024
  - this is maximum size reusable array from ArrayPool
- Test files: 1GB and 10GB

## Design
Based on [External merge sort](https://en.wikipedia.org/wiki/External_sorting#External_merge_sort)

### Phase 0 - Initialization
Split file into chunks which can be processed parallely in Phase 1
- Calculate lines count using [algorithm](https://nimaara.com/2018/03/20/counting-lines-of-a-text-file-in-csharp.html) 