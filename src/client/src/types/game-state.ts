export type GameState = 'Draft' | 'Open' | 'InProgress' | 'Completed';

export abstract class GameStates {
  public static Draft: GameState = 'Draft';
  public static Open: GameState = 'Open';
  public static InProgress: GameState = 'InProgress';
  public static Completed: GameState = 'Completed';
}
