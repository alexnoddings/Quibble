import {ParticipantId, QuestionId, RoundId} from '$/types/ids';

interface Round {
  id: RoundId;
}

interface Question {
  id: QuestionId;
}

interface Answer {
  id: Answer;
}

// Use Round/Question Ids for the Ids since they're stable.
// Sequence/order would be nicer, but is unstable.
export class IdGenerator {
  public static roundTitle(round: Round): string {
    return 'round-' + round.id + '-title';
  }

  public static questionBody(question: Question): string {
    return 'question-' + question.id + '-body';
  }

  public static questionAnswer(question: Question): string {
    return 'question-' + question.id + '-answer';
  }

  public static questionPoints(question: Question): string {
    return 'question-' + question.id + '-points';
  }

  public static answerPointsDialog(questionId: QuestionId, participantId: ParticipantId): string {
    return 'answer-' + questionId + '_' + participantId + '-points';
  }
}
