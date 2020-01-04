# TaigaPlanningPokerListMaker

This is a console app for connecting to the Taiga API and collecting all User Stories and Issues across multiple projects in Taiga Project Management. It creates the following CSVs:
- unassigned_issues : useful for knowing what issues need attention
- assigned_issues : useful for knowing who still may have stale issues
- new unassigned_user_stories: useful list for Poker Planning
- assigned_user_stores (not part of a sprint): useful for knowing "what is coming up" for users who have taken USs that are not yet part of a milestone.
- issues and userstory CSVs for each user who is registered on Taiga. Helpful for tracking progress of each user.

Eventually this will be turned into a Slack Bot.

Plans also include estimating velocity by individual and other metrics for making appropriate estimates to deliverables.
