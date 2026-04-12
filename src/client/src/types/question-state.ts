export type QuestionState = 'Hidden' | 'InProgress' | 'Marking' | 'Revealed';

export abstract class QuestionStates {
  public static Hidden: QuestionState = 'Hidden';
  public static InProgress: QuestionState = 'InProgress';
  public static Marking: QuestionState = 'Marking';
  public static Revealed: QuestionState = 'Revealed';
}
