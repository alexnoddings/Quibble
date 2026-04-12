export type GameRelationship = 'Owner' | 'Participant' | 'None';

export abstract class GameRelationships {
  public static Owner: GameRelationship = 'Owner';
  public static Participant: GameRelationship = 'Participant';
  public static None: GameRelationship = 'None';
}
