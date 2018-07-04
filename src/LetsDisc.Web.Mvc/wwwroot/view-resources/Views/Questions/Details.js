(function() {
    $(function() {

        var _questionService = abp.services.app.question;
        var _session = { user: null };

        abp.services.app.session.getCurrentLoginInformations({
            async: false
        }).done(function (result) {
            if (result) {
                _session.user = result.user;
            }
        });

        $('.vote-up').click(function () {
            var questionId = $(this).attr("data-question-id");
            if (_session.user) {
                upVote(questionId);
            } else {
                abp.notify.info("You need to login for Upvoting");
            }
            
        });

        $('.vote-down').click(function () {
            var questionId = $(this).attr("data-question-id");
            if (_session.user) {
                downVote(questionId);
            } else {
                abp.notify.info("You need to login for Downvoting");
            }

        });


        function upVote(questionId) {
            _questionService.questionVoteUp({
                id: questionId
            }).done(function (data) {
                $('.upvote-count').text(data.voteCount);
                changeClass(data.upVote, data.downVote);
                abp.notify.info("Saved your vote. Thanks.");
            });
        }

        function downVote(questionId) {
            _questionService.questionVoteDown({
                id: questionId
            }).done(function (data) {
                $('.upvote-count').text(data.voteCount);
                changeClass(data.upVote, data.downVote);
                abp.notify.info("Saved your vote. Thanks.");
            });
        }

        function changeClass(upvote, downvote) {
            if (upvote) {
                $('.vote-up').addClass('vote-up-on');
                $('.vote-down').removeClass('vote-down-on');
            } else if (downvote) {
                $('.vote-up').removeClass('vote-up-on');
                $('.vote-down').addClass('vote-down-on');
            } else {
                $('.vote-up').removeClass('vote-up-on');
                $('.vote-down').removeClass('vote-down-on');
            }
        }
    });
})();