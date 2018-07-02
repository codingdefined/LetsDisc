(function() {
    $(function() {

        var _questionService = abp.services.app.question;

        $('.vote-up').click(function () {
            var questionId = $(this).attr("data-question-id");
            upVote(questionId);
        });

        $('.vote-down').click(function () {
            var questionId = $(this).attr("data-question-id");
            downVote(questionId);
        });


        function upVote(questionId) {
            _questionService.questionVoteUp({
                id: questionId
            }).done(function (data) {
                $('.upvote-count').text(data.voteCount);
                abp.notify.info("Saved your vote. Thanks.");
            });
        }

        function downVote(questionId) {
            _questionService.questionVoteDown({
                id: questionId
            }).done(function (data) {
                $('.upvote-count').text(data.voteCount);
                abp.notify.info("Saved your vote. Thanks.");
            });
        }
    });
})();