public enum EGameState { Ready, Play, TimeOver }
public enum EDirection { Up, Right, Down, Left, Center }
public enum ETeam { Red, Blue }

public delegate void VoidVoidDelegate();
public delegate void VoidTeamDelegate(ETeam _team);