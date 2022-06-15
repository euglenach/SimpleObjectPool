# What is "SimpleObjectPool"

A simple ObjectPooling library.

When used in Unity, pure C# classes and MonoBehaviour classes can be used together.  
Of course, it can also be used in pure C#!(Delete any files you don't need...)


Features the ability to customize how instances are generated.

Pure C# Class

```C#
var pool = new ObjectPool<HogeClass>(() => new HogeClass());
```

MonoBehaviour

```C#
var pool = new ObjectPool<HogeMonoBehaviour>(() => Instantiate(hoge));
```

## AsyncObjectPool

using UniTask.

```C#
var pool = new AsyncObjectPool<HogeClass>(() => await HogeFactory.CreateAsync());
await pool.PreLoadAsync();
var obj = await pool.TakeAsync();
```
