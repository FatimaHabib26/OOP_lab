from pathlib import Path
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import mm
from reportlab.pdfgen import canvas
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, KeepTogether

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "output" / "pdf" / "Monster_Battle_Arena_Student_Assignment.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

NAVY = colors.HexColor("#172A46")
BLUE = colors.HexColor("#2563A6")
PALE = colors.HexColor("#EAF2FA")
GOLD = colors.HexColor("#F2B544")
INK = colors.HexColor("#1F2937")
MUTED = colors.HexColor("#5B677A")

styles = getSampleStyleSheet()
styles.add(ParagraphStyle(name="CoverTitle", parent=styles["Title"], fontName="Helvetica-Bold", fontSize=25, leading=30, textColor=NAVY, alignment=TA_CENTER, spaceAfter=12))
styles.add(ParagraphStyle(name="CoverSub", parent=styles["Normal"], fontSize=12, leading=18, textColor=MUTED, alignment=TA_CENTER))
styles.add(ParagraphStyle(name="H1x", parent=styles["Heading1"], fontName="Helvetica-Bold", fontSize=18, leading=22, textColor=NAVY, spaceBefore=4, spaceAfter=10))
styles.add(ParagraphStyle(name="H2x", parent=styles["Heading2"], fontName="Helvetica-Bold", fontSize=12.5, leading=16, textColor=BLUE, spaceBefore=10, spaceAfter=5))
styles.add(ParagraphStyle(name="Bodyx", parent=styles["BodyText"], fontSize=10, leading=15, textColor=INK, spaceAfter=6))
styles.add(ParagraphStyle(name="Smallx", parent=styles["BodyText"], fontSize=8.5, leading=12, textColor=MUTED))
styles.add(ParagraphStyle(name="Task", parent=styles["Heading2"], fontName="Helvetica-Bold", fontSize=15, leading=19, textColor=colors.white, backColor=BLUE, borderPadding=8, spaceBefore=4, spaceAfter=10))
styles.add(ParagraphStyle(name="CodeBox", parent=styles["Code"], fontName="Courier", fontSize=8.5, leading=12, textColor=NAVY, backColor=PALE, borderPadding=7, spaceBefore=4, spaceAfter=7))

class HeaderFooterCanvas(canvas.Canvas):
    """Adds the same header and footer after page content is laid out."""
    def __init__(self, *args, **kwargs):
        canvas.Canvas.__init__(self, *args, **kwargs)
        self._saved_pages = []

    def showPage(self):
        self._saved_pages.append(dict(self.__dict__))
        self._startPage()

    def save(self):
        page_count = len(self._saved_pages)
        for page_number, page_state in enumerate(self._saved_pages, 1):
            self.__dict__.update(page_state)
            self._draw_header_footer(page_number, page_count)
            canvas.Canvas.showPage(self)
        canvas.Canvas.save(self)

    def _draw_header_footer(self, page_number, page_count):
        self.saveState()
        width, height = A4
        self.setFillColor(NAVY)
        self.rect(0, height - 12*mm, width, 12*mm, fill=1, stroke=0)
        self.setFillColor(colors.white)
        self.setFont("Helvetica-Bold", 8)
        self.drawString(18*mm, height - 7.5*mm, "MONSTER BATTLE ARENA | STUDENT ASSIGNMENT")
        self.setFillColor(MUTED)
        self.setFont("Helvetica", 8)
        self.drawString(18*mm, 10*mm, "C# OOP and Layered Architecture")
        self.drawRightString(width - 18*mm, 10*mm, "Page " + str(page_number) + " of " + str(page_count))
        self.restoreState()

def header_footer_unused(canvas, doc):
    canvas.saveState()
    width, height = A4
    canvas.setFillColor(NAVY)
    canvas.rect(0, height - 12*mm, width, 12*mm, fill=1, stroke=0)
    canvas.setFillColor(colors.white)
    canvas.setFont("Helvetica-Bold", 8)
    canvas.drawString(18*mm, height - 7.5*mm, "MONSTER BATTLE ARENA | STUDENT ASSIGNMENT")
    canvas.setFillColor(MUTED)
    canvas.setFont("Helvetica", 8)
    canvas.drawString(18*mm, 10*mm, "C# OOP and Layered Architecture")
    canvas.drawRightString(width - 18*mm, 10*mm, "Page " + str(doc.page))
    canvas.restoreState()

def bullet(text):
    return Paragraph("- " + text, styles["Bodyx"])

doc = SimpleDocTemplate(str(OUTPUT), pagesize=A4, rightMargin=18*mm, leftMargin=18*mm, topMargin=22*mm, bottomMargin=18*mm, title="Monster Battle Arena Student Assignment", author="GameProject")
story = []

story += [Spacer(1, 28*mm), Paragraph("MONSTER BATTLE ARENA", styles["CoverTitle"]), Paragraph("Student Programming Assignment", styles["CoverSub"]), Spacer(1, 10*mm)]
summary = Table([
    ["Language", "C# Console Application"],
    ["Architecture", "Models, Repository, Service, UI, Main"],
    ["Concepts", "Classes, public/private, lists, functions, file handling"],
    ["Restriction", "No inheritance, interfaces, or LINQ required"],
], colWidths=[38*mm, 105*mm])
summary.setStyle(TableStyle([
    ("BACKGROUND", (0,0), (0,-1), NAVY), ("TEXTCOLOR", (0,0), (0,-1), colors.white),
    ("BACKGROUND", (1,0), (1,-1), PALE), ("TEXTCOLOR", (1,0), (1,-1), INK),
    ("FONTNAME", (0,0), (0,-1), "Helvetica-Bold"), ("FONTNAME", (1,0), (1,-1), "Helvetica"),
    ("FONTSIZE", (0,0), (-1,-1), 9.5), ("LEADING", (0,0), (-1,-1), 13),
    ("VALIGN", (0,0), (-1,-1), "MIDDLE"), ("BOX", (0,0), (-1,-1), 0.5, BLUE),
    ("INNERGRID", (0,0), (-1,-1), 0.4, colors.white), ("TOPPADDING", (0,0), (-1,-1), 7), ("BOTTOMPADDING", (0,0), (-1,-1), 7),
]))
story += [summary, Spacer(1, 13*mm), Paragraph("Your mission", styles["H1x"]), Paragraph("Complete the unfinished parts of a working console battle game. Keep every responsibility in its correct layer. Your solution must compile, preserve the existing gameplay, and make the requested features work.", styles["Bodyx"]), Spacer(1, 8*mm), Paragraph("Name: ____________________________    Date: __________________", styles["Bodyx"]), PageBreak()]

story += [Paragraph("1. Understand the project", styles["H1x"]), Paragraph("The application is already separated into layers. Before writing code, trace the program from Program.cs to the UI, service, repository, and models.", styles["Bodyx"])]
layer_data = [["Layer", "Responsibility", "Important rule"], ["Models", "Store player, monster, and result data", "No Console or file code"], ["Repository", "Read and write scores.txt", "Only this layer handles the score file"], ["Service", "Apply battle and scoring rules", "No Console.ReadLine or WriteLine"], ["UI", "Display menus and collect input", "Ask the service to apply rules"], ["Program.cs", "Create objects and start the UI", "Do not place gameplay here"]]
layers = Table(layer_data, colWidths=[27*mm, 72*mm, 54*mm], repeatRows=1)
layers.setStyle(TableStyle([("BACKGROUND", (0,0), (-1,0), NAVY), ("TEXTCOLOR", (0,0), (-1,0), colors.white), ("FONTNAME", (0,0), (-1,0), "Helvetica-Bold"), ("BACKGROUND", (0,1), (-1,-1), colors.white), ("ROWBACKGROUNDS", (0,1), (-1,-1), [colors.white, PALE]), ("GRID", (0,0), (-1,-1), 0.4, colors.HexColor("#AFC3D8")), ("VALIGN", (0,0), (-1,-1), "TOP"), ("FONTSIZE", (0,0), (-1,-1), 8.5), ("LEADING", (0,0), (-1,-1), 11), ("TOPPADDING", (0,0), (-1,-1), 6), ("BOTTOMPADDING", (0,0), (-1,-1), 6)]))
story += [layers, Spacer(1, 7*mm), Paragraph("Expected game flow", styles["H2x"]), bullet("Program.cs creates ScoreRepository, GameService, and GameUI."), bullet("GameUI.Start() displays the main menu."), bullet("The UI creates a player through the service and starts battles."), bullet("The service calculates damage, rewards, and final results."), bullet("The repository saves each completed game in scores.txt."), Spacer(1, 3*mm), Paragraph("Rules for all tasks", styles["H2x"]), bullet("Work only inside the indicated method unless a requirement clearly needs another small change."), bullet("Do not move Console code into the service or file code into the UI."), bullet("Do not remove the existing comments or working game features."), bullet("Use clear variable names and add short comments for non-obvious logic."), PageBreak()]

story += [Paragraph("Task 1 - Complete the instructions screen", styles["Task"]), Paragraph("File: UI/GameUI.cs | Method: ShowInstructions() | Difficulty: Basic", styles["Smallx"]), Paragraph("Replace the placeholder message with a useful How to Play screen.", styles["Bodyx"]), Paragraph("Requirements", styles["H2x"]), bullet("Clear the console before displaying the instructions."), bullet("Explain how the player attacks and uses healing potions."), bullet("Explain how the player wins or loses."), bullet("Explain that defeated monsters award points and winning may award a bonus."), bullet("Call Pause() after the instructions so the user can read them."), Paragraph("Questions to consider", styles["H2x"]), bullet("Which layer should contain Console.Clear and Console.WriteLine?"), bullet("Why should this method not calculate damage or modify health?"), Paragraph("Example shape - do not copy blindly", styles["H2x"]), Paragraph("private void ShowInstructions()<br/>{<br/>    // Clear screen<br/>    // Print several instruction lines<br/>    // Pause<br/>}", styles["CodeBox"]), Spacer(1, 5*mm), Paragraph("Task 2 - Calculate the victory bonus", styles["Task"]), Paragraph("Implement this task in: Services/GameService.cs | Function: CalculateVictoryBonus(Player player) | Difficulty: Intermediate", styles["Smallx"]), Paragraph("When the player defeats every monster, award bonus points for the health they still have and for every unused healing potion. Each unused potion should be worth ten additional points. The function must calculate and return the complete victory bonus.", styles["Bodyx"]), Paragraph("Requirements", styles["H2x"]), bullet("Use the Player object received by the function."), bullet("Use both the player's remaining health and unused healing potions."), bullet("Return an int; do not print the bonus in the service."), bullet("Do not change the player's score inside this function. The UI already adds the returned bonus."), Paragraph("Questions to consider", styles["H2x"]), bullet("Why does the function return a value instead of directly printing it?"), bullet("What would happen if both this function and the UI added the bonus to Score?"), PageBreak()]

story += [Spacer(1, 14*mm), Paragraph("Task 3 - Build the top-five leaderboard", styles["Task"]), Paragraph("File: Services/GameService.cs | Method: GetTopScores(int numberOfScores) | Difficulty: Advanced", styles["Smallx"]), Paragraph("The current fallback returns every saved result without sorting. Replace it with a manually sorted leaderboard. Do not use LINQ for this exercise.", styles["Bodyx"]), Paragraph("Requirements", styles["H2x"]), bullet("Call _repository.GetAll() to obtain all saved GameResult objects."), bullet("Create a new list so sorting does not unexpectedly change another list."), bullet("Sort results from highest Score to lowest Score using loops."), bullet("When scores are equal, sort PlayerName alphabetically from A to Z."), bullet("Return at most numberOfScores items."), bullet("If fewer results exist, return all available results without an error."), bullet("If no scores exist, return an empty list."), Paragraph("Suggested algorithm", styles["H2x"]), bullet("Copy each result into a working list."), bullet("Use an outer loop to select a position and an inner loop to find a better result."), bullet("A result is better if its score is higher, or if scores match and its name comes first alphabetically."), bullet("Swap objects using a temporary GameResult variable."), bullet("Copy only the first requested items into the list that you return."), Paragraph("Tie comparison hint", styles["H2x"]), Paragraph("string.Compare(first.PlayerName, second.PlayerName, true) &lt; 0", styles["CodeBox"]), Paragraph("The expression is true when the first name should appear before the second name, ignoring uppercase/lowercase differences.", styles["Smallx"]), Paragraph("Example expected order", styles["H2x"])]
sample = Table([["Player", "Score"], ["Aisha", "120"], ["Bilal", "120"], ["Sara", "95"], ["Hamza", "70"]], colWidths=[70*mm, 35*mm])
sample.setStyle(TableStyle([("BACKGROUND", (0,0), (-1,0), NAVY), ("TEXTCOLOR", (0,0), (-1,0), colors.white), ("FONTNAME", (0,0), (-1,0), "Helvetica-Bold"), ("GRID", (0,0), (-1,-1), .4, colors.HexColor("#AFC3D8")), ("ALIGN", (1,1), (1,-1), "RIGHT"), ("FONTSIZE", (0,0), (-1,-1), 9), ("TOPPADDING", (0,0), (-1,-1), 5), ("BOTTOMPADDING", (0,0), (-1,-1), 5)]))
story += [sample, PageBreak()]

story += [Paragraph("Testing checklist", styles["H1x"]), Paragraph("Build and run the program after each task. Use the checklist below before submitting.", styles["Bodyx"])]
tests = [["Test", "Expected result"], ["Build project", "0 errors; ideally 0 warnings"], ["Menu option 3", "Clear and complete instructions appear, then pause"], ["Win with health and potions remaining", "Both values contribute to the victory bonus"], ["No score file exists", "Leaderboard opens without crashing"], ["Only 3 results exist; request 5", "Exactly 3 results are returned"], ["More than 5 results exist", "Only the highest 5 are displayed"], ["Two results share a score", "Names are ordered A-Z"], ["Scores: 10, 90, 30", "Displayed order is 90, 30, 10"]]
test_table = Table(tests, colWidths=[65*mm, 88*mm], repeatRows=1)
test_table.setStyle(TableStyle([("BACKGROUND", (0,0), (-1,0), NAVY), ("TEXTCOLOR", (0,0), (-1,0), colors.white), ("FONTNAME", (0,0), (-1,0), "Helvetica-Bold"), ("ROWBACKGROUNDS", (0,1), (-1,-1), [colors.white, PALE]), ("GRID", (0,0), (-1,-1), .4, colors.HexColor("#AFC3D8")), ("VALIGN", (0,0), (-1,-1), "TOP"), ("FONTSIZE", (0,0), (-1,-1), 8.5), ("LEADING", (0,0), (-1,-1), 11), ("TOPPADDING", (0,0), (-1,-1), 6), ("BOTTOMPADDING", (0,0), (-1,-1), 6)]))
story += [test_table, Spacer(1, 8*mm), Paragraph("Submission checklist", styles["H2x"]), bullet("ShowInstructions() is completed."), bullet("CalculateVictoryBonus() returns the correct result."), bullet("GetTopScores() meets all five leaderboard rules."), bullet("The code remains in the correct layers."), bullet("The full game builds and runs."), Spacer(1, 7*mm)]
grading = Table([["Area", "Points"], ["Instructions screen", "15"], ["Victory bonus", "20"], ["Leaderboard sorting and limiting", "40"], ["Layer separation and code quality", "15"], ["Testing and working build", "10"], ["Total", "100"]], colWidths=[105*mm, 30*mm])
grading.setStyle(TableStyle([("BACKGROUND", (0,0), (-1,0), NAVY), ("TEXTCOLOR", (0,0), (-1,0), colors.white), ("BACKGROUND", (0,-1), (-1,-1), GOLD), ("FONTNAME", (0,0), (-1,0), "Helvetica-Bold"), ("FONTNAME", (0,-1), (-1,-1), "Helvetica-Bold"), ("GRID", (0,0), (-1,-1), .4, colors.HexColor("#AFC3D8")), ("ALIGN", (1,1), (1,-1), "CENTER"), ("FONTSIZE", (0,0), (-1,-1), 9), ("TOPPADDING", (0,0), (-1,-1), 5), ("BOTTOMPADDING", (0,0), (-1,-1), 5)]))
story += [Paragraph("Suggested marking guide", styles["H2x"]), grading]

doc.build(story, canvasmaker=HeaderFooterCanvas)
print(OUTPUT)
