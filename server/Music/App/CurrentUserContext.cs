﻿namespace Music.App
{
    public interface ICurrentUserContext
    {
        int Id { get; }
    }
    public class CurrentUserContextMock : ICurrentUserContext
    {
        public int Id => 1;
    }
}
