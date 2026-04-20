using KanaLearning.Core.Services;

namespace KanaLearning.Tests;

[TestClass]
public sealed class KanaMapperTests
{
    [TestMethod]
    public void GetRomaji_SingleKana_ReturnsCorrectRomaji()
    {
        Assert.AreEqual("a", KanaMapper.GetRomaji("あ"));
        Assert.AreEqual("ki", KanaMapper.GetRomaji("キ"));
    }

    [TestMethod]
    public void GetRomaji_MultipleKana_ReturnsCorrectRomajiWord()
    {
        Assert.AreEqual("arigatou", KanaMapper.GetRomaji("ありがとう"));
        Assert.AreEqual("watashi", KanaMapper.GetRomaji("わたし"));
    }

    [TestMethod]
    public void GetRomaji_Yoon_ReturnsCorrectRomaji()
    {
        Assert.AreEqual("kya", KanaMapper.GetRomaji("きゃ"));
        Assert.AreEqual("ryu", KanaMapper.GetRomaji("リュ"));
    }

    [TestMethod]
    public void GetRomaji_SmallTsu_DoublesNextConsonant()
    {
        Assert.AreEqual("kippu", KanaMapper.GetRomaji("きっぷ"));
        Assert.AreEqual("zasshi", KanaMapper.GetRomaji("ざっし"));
        Assert.AreEqual("kocchi", KanaMapper.GetRomaji("こっち"));
    }

    [TestMethod]
    public void GetRomaji_LongVowelMark_ReturnsDash()
    {
        Assert.AreEqual("su-pa-", KanaMapper.GetRomaji("スーパー"));
        Assert.AreEqual("ko-hi-", KanaMapper.GetRomaji("コーヒー"));
    }

    [TestMethod]
    public void GetRomaji_WithSpaces_PreservesSpaces()
    {
        Assert.AreEqual("ohayou gozaimasu", KanaMapper.GetRomaji("おはよう ございます"));
    }

    [TestMethod]
    public void GetRomaji_InvalidOrMixedCharacters_ReturnsNull()
    {
        Assert.IsNull(KanaMapper.GetRomaji("漢字"));
        Assert.IsNull(KanaMapper.GetRomaji("hello"));
        Assert.IsNull(KanaMapper.GetRomaji("あa"));
    }

    [TestMethod]
    public void GetRomaji_SmallTsuAtEnd_ReturnsSafelyWithoutDoubling()
    {
        // "あっ" -> sets doubleNextConsonant but safely terminates loop, resulting in "a"
        Assert.AreEqual("a", KanaMapper.GetRomaji("あっ"));
    }
}
