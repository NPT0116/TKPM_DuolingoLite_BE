using System;

namespace Domain.Entities.Ranking;



// ranking dùng để chứa thông tin về thứ hạng của người dùng trong hệ thống
// thứ hạng được tính dựa trên số điểm kinh nghiệm của người dùng
public class Ranking
{
    public int Top { get; set; } = 10;
    public Ranking(int Top)
    {
        this.Top = Top;
    }
    public Ranking()
    {
        
    }
    public List<UserRanking> UserRankings { get; set; } = new List<UserRanking>();

}

// user ranking dùng để chứa thông tin về thứ hạng của người dùng và thông tin người dùng đó
// thứ hạng được tính dựa trên số điểm kinh nghiệm của người dùng
public class UserRanking {
    public Guid UserId { get; set; }
    public int Rank { get; set; }
    public string NickName { get; set; }
    public int ExperiencePoint { get; set; }

    public UserRanking(Guid userId, int rank, string nickName, int experiencePoint)
    {
        UserId = userId;
        Rank = rank;
        NickName = nickName;
        ExperiencePoint = experiencePoint;
    }

}